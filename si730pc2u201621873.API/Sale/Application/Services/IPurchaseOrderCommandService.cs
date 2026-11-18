using si730pc2u201621873.API.Sale.Application.Errors;
using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Domain.Model.Commands;
using si730pc2u201621873.API.Shared.Application.Patterns;

namespace si730pc2u201621873.API.Sale.Application.Services;

/// <summary>
///     Command service interface for purchase order operations.
/// </summary>
/// <remarks>
///     This interface defines the contract for handling purchase order creation commands.
///     It ensures duplicate detection and persistence of new purchase orders.
/// </remarks>
public interface IPurchaseOrderCommandService
{
    /// <summary>
    ///     Handle the create purchase order command.
    /// </summary>
    /// <remarks>
    ///     This method checks if a purchase order already exists for the given Customer and PoNumber.
    ///     If it does not exist, it creates a new purchase order and adds it to the database.
    /// </remarks>
    /// <param name="command">CreatePurchaseOrderCommand command</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A Result containing the created PurchaseOrder on success, or the error reason on failure.
    /// </returns>
    Task<Result<PurchaseOrder, CreatePurchaseOrderError>> Handle(CreatePurchaseOrderCommand command,
        CancellationToken cancellationToken = default);
}
