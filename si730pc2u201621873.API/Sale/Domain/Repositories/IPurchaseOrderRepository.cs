using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;
using si730pc2u201621873.API.Shared.Domain.Repositories;

namespace si730pc2u201621873.API.Sale.Domain.Repositories;

/// <summary>
///     The purchase order repository contract.
/// </summary>
public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder>
{
    /// <summary>
    ///     Find a purchase order by customer and P.O. Number
    /// </summary>
    /// <param name="customer">The customer</param>
    /// <param name="poNumber">The P.O. Number</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     The purchase order object if found, or null otherwise.
    /// </returns>
    Task<PurchaseOrder?> FindByCustomerAndPoNumberAsync(Customer customer, PoNumber poNumber,
        CancellationToken cancellationToken = default);
}
