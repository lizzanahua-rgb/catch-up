namespace si730pc2u201621873.API.Sale.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a purchase order.
/// </summary>
/// <remarks>
///     Audit fields (createdAt, updatedAt) are intentionally not exposed.
/// </remarks>
/// <param name="Id">The server-generated ID of the purchase order</param>
/// <param name="Customer">The customer that issued the purchase order</param>
/// <param name="PoNumber">The P.O. Number handled internally by the customer</param>
/// <param name="FabricId">The fabric identifier</param>
/// <param name="Vendor">The vendor</param>
/// <param name="ShipTo">The shipping destination</param>
/// <param name="Quantity">The ordered quantity</param>
public record PurchaseOrderResource(
    int Id,
    string Customer,
    string PoNumber,
    int FabricId,
    string Vendor,
    string ShipTo,
    int Quantity);
