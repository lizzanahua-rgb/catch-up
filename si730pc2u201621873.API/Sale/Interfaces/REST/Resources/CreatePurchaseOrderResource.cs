using System.ComponentModel.DataAnnotations;

namespace si730pc2u201621873.API.Sale.Interfaces.REST.Resources;

/// <summary>
///     Represents the data required to create a purchase order.
/// </summary>
/// <param name="Customer">The customer that issues the purchase order (required, 4 to 50 characters)</param>
/// <param name="PoNumber">The P.O. Number handled internally by the customer (required)</param>
/// <param name="FabricId">The fabric identifier from 1 to 8 (optional, Algodon when omitted)</param>
/// <param name="Vendor">The vendor (required, 4 to 50 characters)</param>
/// <param name="ShipTo">The shipping destination (required)</param>
/// <param name="Quantity">The ordered quantity (required)</param>
public record CreatePurchaseOrderResource(
    [Required(ErrorMessage = "RequiredCustomer")]
    string Customer,
    [Required(ErrorMessage = "RequiredPoNumber")]
    string PoNumber,
    int? FabricId,
    [Required(ErrorMessage = "RequiredVendor")]
    string Vendor,
    [Required(ErrorMessage = "RequiredShipTo")]
    string ShipTo,
    [Required(ErrorMessage = "RequiredQuantity")]
    int? Quantity);
