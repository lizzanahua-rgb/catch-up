using si730pc2u201621873.API.Sale.Domain.Model.Commands;
using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;

namespace si730pc2u201621873.API.Sale.Domain.Model.Aggregates;

/// <summary>
///     Represents a purchase order requested by a customer to Hialpesa.
/// </summary>
/// <remarks>
///     This aggregate root belongs to the Sale bounded context.
///     The combination of (Customer, PoNumber) must be unique within the system.
///     When no fabric is specified the default fabric (Algodon) is assigned.
///     Audit timestamps (CreatedAt, UpdatedAt) are managed by the persistence layer.
///     Author: NOMBRES Y APELLIDOS DEL ESTUDIANTE
/// </remarks>
public partial class PurchaseOrder
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected PurchaseOrder()
    {
        Customer = null!;
        PoNumber = null!;
        FabricId = null!;
        Vendor = null!;
        ShipTo = null!;
    }

    /// <summary>
    ///     Creates a new PurchaseOrder from a creation command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreatePurchaseOrderCommand.
    ///     It applies the business rule that assigns Algodon when no fabric is provided.
    /// </remarks>
    /// <param name="command">The CreatePurchaseOrderCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public PurchaseOrder(CreatePurchaseOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        Customer = command.Customer;
        PoNumber = command.PoNumber;
        FabricId = command.FabricId ?? FabricId.Default;
        Vendor = command.Vendor;
        ShipTo = command.ShipTo;
        Quantity = command.Quantity;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this purchase order.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the customer that issued this purchase order.
    /// </summary>
    public Customer Customer { get; private set; }

    /// <summary>
    ///     Gets the P.O. Number handled internally by the customer.
    /// </summary>
    public PoNumber PoNumber { get; private set; }

    /// <summary>
    ///     Gets the fabric requested in this purchase order.
    /// </summary>
    public FabricId FabricId { get; private set; }

    /// <summary>
    ///     Gets the vendor of this purchase order.
    /// </summary>
    public Vendor Vendor { get; private set; }

    /// <summary>
    ///     Gets the shipping destination of this purchase order.
    /// </summary>
    public ShipTo ShipTo { get; private set; }

    /// <summary>
    ///     Gets the ordered quantity.
    /// </summary>
    public int Quantity { get; private set; }
}
