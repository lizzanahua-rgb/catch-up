using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;
using si730pc2u201621873.API.Sale.Domain.Repositories;
using si730pc2u201621873.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using si730pc2u201621873.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace si730pc2u201621873.API.Sale.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for purchase order persistence.
/// </summary>
/// <remarks>
///     This repository extends the generic BaseRepository with purchase-order-specific query methods.
///     Author: NOMBRES Y APELLIDOS DEL ESTUDIANTE
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class PurchaseOrderRepository(AppDbContext context)
    : BaseRepository<PurchaseOrder>(context), IPurchaseOrderRepository
{
    /// <inheritdoc />
    public async Task<PurchaseOrder?> FindByCustomerAndPoNumberAsync(Customer customer, PoNumber poNumber,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<PurchaseOrder>()
            .FirstOrDefaultAsync(p => p.Customer == customer && p.PoNumber == poNumber,
                cancellationToken);
    }
}
