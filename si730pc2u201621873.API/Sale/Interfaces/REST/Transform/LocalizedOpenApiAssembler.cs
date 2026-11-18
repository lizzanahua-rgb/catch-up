using si730pc2u201621873.API.Sale.Interfaces.REST.Resources;
using si730pc2u201621873.API.Resources;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;

namespace si730pc2u201621873.API.Sale.Interfaces.REST.Transform;

/// <summary>
///     Assembles localized OpenAPI metadata (summaries, descriptions, and response texts)
///     for the Sale REST endpoints and resources.
/// </summary>
public static class LocalizedOpenApiAssembler
{
    private static readonly Dictionary<string, (string Summary, string Description,
        IReadOnlyDictionary<string, string> Responses)> OperationResources =
        new Dictionary<string, (string, string, IReadOnlyDictionary<string, string>)>
        {
            ["CreatePurchaseOrder"] = (
                "CreatePurchaseOrderSummary",
                "CreatePurchaseOrderDescription",
                new Dictionary<string, string>
                {
                    ["201"] = "CreatePurchaseOrderCreated",
                    ["400"] = "CreatePurchaseOrderBadRequest",
                    ["409"] = "CreatePurchaseOrderConflict",
                    ["500"] = "CreatePurchaseOrderServerError"
                })
        };

    /// <summary>
    ///     Localizes the summary, description, operation id and response descriptions of known operations.
    /// </summary>
    /// <param name="operation">The OpenAPI operation being transformed.</param>
    /// <param name="context">The transformer context, used to resolve the localizer and action descriptor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed task.</returns>
    public static Task TransformOperationAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action ||
            !OperationResources.TryGetValue(action.ActionName, out var resources))
            return Task.CompletedTask;

        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        operation.OperationId = action.ActionName;
        operation.Summary = localizer[resources.Summary].Value;
        operation.Description = localizer[resources.Description].Value;
        if (operation.Responses is null) return Task.CompletedTask;

        foreach (var (statusCode, resourceKey) in resources.Responses)
        {
            if (operation.Responses.TryGetValue(statusCode, out var response) &&
                response is OpenApiResponse openApiResponse)
                openApiResponse.Description = localizer[resourceKey].Value;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Localizes the description of known resource schemas and their properties.
    /// </summary>
    /// <param name="schema">The OpenAPI schema being transformed.</param>
    /// <param name="context">The transformer context, used to resolve the localizer and type information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed task.</returns>
    public static Task TransformSchemaAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var type = context.JsonTypeInfo.Type;
        if (type == typeof(CreatePurchaseOrderResource))
            schema.Description = localizer["CreatePurchaseOrderSchema"].Value;
        else if (type == typeof(PurchaseOrderResource))
            schema.Description = localizer["PurchaseOrderSchema"].Value;

        var propertyResourceKey = context.JsonPropertyInfo?.Name switch
        {
            "id" => "PurchaseOrderId",
            "customer" => "PurchaseOrderCustomer",
            "poNumber" => "PurchaseOrderPoNumber",
            "fabricId" => "PurchaseOrderFabricId",
            "vendor" => "PurchaseOrderVendor",
            "shipTo" => "PurchaseOrderShipTo",
            "quantity" => "PurchaseOrderQuantity",
            _ => null
        };

        if (propertyResourceKey is not null)
            schema.Description = localizer[propertyResourceKey].Value;

        return Task.CompletedTask;
    }
}