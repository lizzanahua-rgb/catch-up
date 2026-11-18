using si730pc2u201621873.API.Sale.Application.Errors;
using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Resources;
using si730pc2u201621873.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace si730pc2u201621873.API.Sale.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from the result of the command handling to create a purchase order in the application layer.
/// </summary>
public static class ActionResultFromCreatePurchaseOrderResultAssembler
{
    /// <summary>
    ///     Assembles an <see cref="ActionResult" /> from the result of a create purchase order operation.
    /// </summary>
    /// <param name="result">The application result produced by the application layer after handling the command to create a purchase order.</param>
    /// <param name="controller">The controller used to produce HTTP-specific action results.</param>
    /// <param name="localizer">The string localizer used for API response messages.</param>
    /// <returns>201 with the resource on success, 409 on duplicates, 500 on unexpected errors.</returns>
    public static ActionResult ToActionResultFromCreatePurchaseOrderResult(
        Result<PurchaseOrder, CreatePurchaseOrderError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<PurchaseOrder, CreatePurchaseOrderError>.Success success =>
                controller.StatusCode(StatusCodes.Status201Created,
                    PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<PurchaseOrder, CreatePurchaseOrderError>.Failure failure =>
                failure.Error switch
                {
                    CreatePurchaseOrderError.DuplicatePurchaseOrder =>
                        controller.Conflict(localizer["PurchaseOrderDuplicated"].Value),

                    CreatePurchaseOrderError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingPurchaseOrder"].Value,
                            statusCode: 500),

                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
