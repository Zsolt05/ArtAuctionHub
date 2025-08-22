using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArtAuctionHub.API.Filters;

/// <summary>
/// An action filter that validates the model state before executing an action.
/// If the model state is invalid, it throws a ValidationException
/// with detailed error information.
/// </summary>
public sealed class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // Extract validation errors from the ModelState.
            var errors = context.ModelState
                .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            // Throw a ValidationException with the collected errors.
            var ex = new ValidationException("Request validation failed.");
            ex.Data["errors"] = errors;

            throw ex;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}