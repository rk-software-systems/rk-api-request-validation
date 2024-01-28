using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RKSoftware.Packages.ApiRequestValidation;

/// <summary>
/// Validate API request path parameters, body and form.
/// Returns NotFound, if one of API request path parameter is null.
/// Returns ValidationProblem, if API request body or form is not valid.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiRequestValidationAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Validate API request path parameters, body and form.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        ArgumentNullException.ThrowIfNull(next, nameof(next));

        if (!context.ArePathParametersValid())
        {
            context.Result = ((ControllerBase)context.Controller).NotFound();
        }
        else if (!await context.AreBodyFormAndQueryParametersValid())
        {
            context.Result = ((ControllerBase)context.Controller).ValidationProblem();
        }
        else
        {
            await next();
        }
    }
}
