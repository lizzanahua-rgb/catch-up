using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

namespace si730pc2u201621873.API.Sale.Domain.Model.Commands;

/// <summary>
///     Command to create a purchase order.
/// </summary>
/// <param name="Customer">The customer that issues the purchase order</param>
/// <param name="PoNumber">The P.O. Number handled internally by the customer</param>
/// <param name="FabricId">The requested fabric, or null to use the default fabric (Algodon)</param>
/// <param name="Vendor">The vendor of the purchase order</param>
/// <param name="ShipTo">The shipping destination</param>
/// <param name="Quantity">The ordered quantity</param>
public record CreatePurchaseOrderCommand(
    Customer Customer,
    PoNumber PoNumber,
    FabricId? FabricId,
    Vendor Vendor,
    ShipTo ShipTo,
    int Quantity);
