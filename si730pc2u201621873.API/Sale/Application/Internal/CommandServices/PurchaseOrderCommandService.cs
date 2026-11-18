using si730pc2u201621873.API.Sale.Application.Errors;
using si730pc2u201621873.API.Sale.Application.Services;
using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Domain.Model.Commands;
using si730pc2u201621873.API.Sale.Domain.Repositories;
using si730pc2u201621873.API.Shared.Application.Patterns;
using si730pc2u201621873.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace si730pc2u201621873.API.Sale.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling purchase order creation commands.
/// </summary>
/// <remarks>
///     This service acts as the command handler for creating new purchase orders.
///     It enforces duplicate detection (Customer + PoNumber) through both application-level checks and
///     database constraints, then coordinates with the unit of work to persist changes.
///     Author: NOMBRES Y APELLIDOS DEL ESTUDIANTE
/// </remarks>
/// <param name="purchaseOrderRepository">Repository for accessing purchase order data.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class PurchaseOrderCommandService(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ILogger<PurchaseOrderCommandService> logger)
    : IPurchaseOrderCommandService
{
    /// <inheritdoc />
    public async Task<Result<PurchaseOrder, CreatePurchaseOrderError>> Handle(CreatePurchaseOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingPurchaseOrder =
            await purchaseOrderRepository.FindByCustomerAndPoNumberAsync(command.Customer, command.PoNumber,
                cancellationToken);
        if (existingPurchaseOrder != null)
        {
            logger.LogWarning(
                "Duplicate purchase order rejected for Customer {Customer} and PoNumber {PoNumber}",
                command.Customer,
                command.PoNumber);
            return new Result<PurchaseOrder, CreatePurchaseOrderError>.Failure(
                CreatePurchaseOrderError.DuplicatePurchaseOrder);
        }

        try
        {
            var purchaseOrder = new PurchaseOrder(command);
            await purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<PurchaseOrder, CreatePurchaseOrderError>.Success(purchaseOrder);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex,
                "Duplicate key violation creating purchase order for Customer {Customer} and PoNumber {PoNumber}",
                command.Customer,
                command.PoNumber);
            return new Result<PurchaseOrder, CreatePurchaseOrderError>.Failure(
                CreatePurchaseOrderError.DuplicatePurchaseOrder);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex,
                "Database update failed creating purchase order for Customer {Customer} and PoNumber {PoNumber}",
                command.Customer,
                command.PoNumber);
            return new Result<PurchaseOrder, CreatePurchaseOrderError>.Failure(
                CreatePurchaseOrderError.UnexpectedError);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating purchase order for Customer {Customer} and PoNumber {PoNumber}",
                command.Customer,
                command.PoNumber);
            return new Result<PurchaseOrder, CreatePurchaseOrderError>.Failure(
                CreatePurchaseOrderError.UnexpectedError);
        }
    }

    /// <summary>
    ///     Determines whether a DbUpdateException represents a duplicate key constraint violation.
    /// </summary>
    /// <param name="exception">The exception to inspect.</param>
    /// <returns>True if the exception is due to a MySQL duplicate key error (code 1062), false otherwise.</returns>
    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062)
                return true;
        }

        return false;
    }
}
