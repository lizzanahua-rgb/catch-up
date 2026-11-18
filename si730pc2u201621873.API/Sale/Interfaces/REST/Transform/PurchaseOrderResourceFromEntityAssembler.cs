using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Interfaces.REST.Resources;

namespace si730pc2u201621873.API.Sale.Interfaces.REST.Transform;

/// <summary>
///     Assembles a PurchaseOrderResource from a PurchaseOrder.
/// </summary>
public static class PurchaseOrderResourceFromEntityAssembler
{
    /// <summary>
    ///     Assembles a PurchaseOrderResource from a PurchaseOrder.
    /// </summary>
    /// <param name="entity">The PurchaseOrder entity</param>
    /// <returns>
    ///     A PurchaseOrderResource assembled from the PurchaseOrder (audit fields excluded)
    /// </returns>
    public static PurchaseOrderResource ToResourceFromEntity(PurchaseOrder entity) =>
        new(entity.Id,
            entity.Customer.Value,
            entity.PoNumber.Value,
            entity.FabricId.Value,
            entity.Vendor.Value,
            entity.ShipTo.Value,
            entity.Quantity);
}
