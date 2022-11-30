using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RKSoftware.Packages.ApiRequestValidation
{
    /// <summary>
    /// Validate API request path parameters, body and form.
    /// Returns NotFound, if one of API request path parameter is null.
    /// Returns ValidationProblem, if API request body or form is not valid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ApiRequestValidationAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (!context.ArePathParametersValid())
            {
                context.Result = ((ControllerBase)context.Controller).NotFound();
            }
            else if (!await context.IsBodyOrFormParameterValid())
            {
                context.Result = ((ControllerBase)context.Controller).ValidationProblem();
            }
            else
            {
                await next();
            }
        }
    }
}
