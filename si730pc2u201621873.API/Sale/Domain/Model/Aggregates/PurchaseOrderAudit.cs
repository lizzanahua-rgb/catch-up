using si730pc2u201621873.API.Shared.Domain.Model;

namespace si730pc2u201621873.API.Sale.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for PurchaseOrder aggregate.
/// </summary>
/// <remarks>
///     This partial class extends PurchaseOrder with audit trail properties.
///     CreatedAt and UpdatedAt are automatically managed by the persistence layer
///     via the AuditableEntityInterceptor in the Shared bounded context.
///     They are stored (columns created_at, updated_at) but never exposed in API responses.
/// </remarks>
public partial class PurchaseOrder : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when this purchase order was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when this purchase order was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
