namespace si730pc2u201621873.API.Sale.Application.Errors;

/// <summary>
///     Represents errors that can occur when creating a purchase order.
/// </summary>
public enum CreatePurchaseOrderError
{
    /// <summary>
    ///     A purchase order with the same customer and P.O. Number already exists.
    /// </summary>
    DuplicatePurchaseOrder,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
