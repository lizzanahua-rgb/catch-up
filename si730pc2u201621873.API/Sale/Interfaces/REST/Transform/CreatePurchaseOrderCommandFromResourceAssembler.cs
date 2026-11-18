using si730pc2u201621873.API.Sale.Domain.Model.Commands;
using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;
using si730pc2u201621873.API.Sale.Interfaces.REST.Resources;

namespace si730pc2u201621873.API.Sale.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CreatePurchaseOrderCommand from a CreatePurchaseOrderResource.
/// </summary>
public static class CreatePurchaseOrderCommandFromResourceAssembler
{
    /// <summary>
    ///     Assembles a CreatePurchaseOrderCommand from a CreatePurchaseOrderResource.
    /// </summary>
    /// <param name="resource">The CreatePurchaseOrderResource resource</param>
    /// <returns>
    ///     A CreatePurchaseOrderCommand assembled from the CreatePurchaseOrderResource
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when any value object rejects its raw value.</exception>
    public static CreatePurchaseOrderCommand ToCommandFromResource(CreatePurchaseOrderResource resource) =>
        new(new Customer(resource.Customer),
            new PoNumber(resource.PoNumber),
            resource.FabricId.HasValue ? new FabricId(resource.FabricId.Value) : null,
            new Vendor(resource.Vendor),
            new ShipTo(resource.ShipTo),
            resource.Quantity ?? throw new ArgumentException("Quantity is required."));
}
