using System.Net.Mime;
using si730pc2u201621873.API.Sale.Application.Services;
using si730pc2u201621873.API.Sale.Interfaces.REST.Resources;
using si730pc2u201621873.API.Sale.Interfaces.REST.Transform;
using si730pc2u201621873.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace si730pc2u201621873.API.Sale.Interfaces.REST;

/// <summary>
///     Purchase orders' controller.
/// </summary>
/// <remarks>
///     Exposes the endpoint /api/v1/purchase-orders (POST only).
///     Author: NOMBRES Y APELLIDOS DEL ESTUDIANTE
/// </remarks>
/// <param name="purchaseOrderCommandService">The Purchase Order Command Service</param>
/// <param name="localizer">String localizer for response messages</param>
/// <param name="logger">Logger for diagnostics and error reporting</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Purchase Orders")]
public class PurchaseOrdersController(
    IPurchaseOrderCommandService purchaseOrderCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<PurchaseOrdersController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Creates a purchase order.
    /// </summary>
    /// <param name="resource">CreatePurchaseOrderResource resource</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     201 with the created purchase order; 400 when a value is invalid; 409 when the customer and
    ///     P.O. Number already exist; 500 on unexpected errors.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var createPurchaseOrderCommand =
                CreatePurchaseOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await purchaseOrderCommandService.Handle(createPurchaseOrderCommand, cancellationToken);
            return ActionResultFromCreatePurchaseOrderResultAssembler.ToActionResultFromCreatePurchaseOrderResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Validation failed while creating purchase order for Customer {Customer} and PoNumber {PoNumber}",
                resource.Customer,
                resource.PoNumber);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while creating purchase order for Customer {Customer} and PoNumber {PoNumber}",
                resource.Customer,
                resource.PoNumber);

            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorCreatingPurchaseOrder"].Value,
                statusCode: 500);
        }
    }
}
