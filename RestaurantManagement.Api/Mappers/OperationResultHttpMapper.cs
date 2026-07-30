using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Services;

namespace RestaurantManagement.Api.Mappers;

public static class OperationResultHttpMapper
{
    public static ActionResult ToErrorResult(
        this ControllerBase controller,
        OperationFailure failure,
        string? message) =>
        failure switch
        {
            OperationFailure.Validation => controller.BadRequest(message),
            OperationFailure.NotFound => controller.NotFound(message),
            OperationFailure.Conflict => controller.Conflict(message),
            _ => controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                "No se pudo completar la operación.")
        };
}
