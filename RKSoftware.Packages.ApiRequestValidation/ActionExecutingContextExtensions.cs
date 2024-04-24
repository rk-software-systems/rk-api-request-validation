using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation;

/// <summary>
/// Extensions for <see cref="ActionExecutingContext"/>.
/// </summary>
public static class ActionExecutingContextExtensions
{
    /// <summary>
    /// Validate request path parameters.
    /// </summary>
    /// <param name="context">A context for action filters.</param>
    /// <returns>Returns false, if one of request path parameters is null.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool ArePathParametersValid(this ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var parameters = context
             .ActionDescriptor
             .Parameters?
             .Where(x => x.BindingInfo?.BindingSource == BindingSource.Path)
             .ToList();

        var flag = true;

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                if (!context.ActionArguments.TryGetValue(parameter.Name, out object? value) || value == null)
                {
                    flag = false;
                    break;
                }
            }
        }

        return flag;
    }

    /// <summary>
    /// Validate API request body or form by using fluent validation validators.
    /// </summary>
    /// <param name="context">A context for action filters.</param>
    /// <param name="cancellation">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns true, if API request body or form is valid.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<bool> AreBodyFormAndQueryModelsValid(this ActionExecutingContext context, CancellationToken cancellation = new CancellationToken())
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var parameters = context
            .ActionDescriptor
            .Parameters?
            .Where(x => (x.BindingInfo?.BindingSource == BindingSource.Body ||
                         x.BindingInfo?.BindingSource == BindingSource.Form ||
                         x.BindingInfo?.BindingSource == BindingSource.Query) &&
                        x.ParameterType.FullName?.StartsWith($"{nameof(System)}.", StringComparison.Ordinal) == false)
            .ToList();

        var flag = true;
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                if (!context.ActionArguments.TryGetValue(parameter.Name, out object? model) || model == null)
                {
                    context.ModelState.AddModelError("", $"{parameter.BindingInfo?.BindingSource?.Id} object is null.");
                    flag = false;
                }
                else
                {
                    var validatorType = typeof(IValidator<>);
                    var genericType = validatorType.MakeGenericType(model.GetType());

                    var validator = context.HttpContext.RequestServices.GetService(genericType);
                    if (validator != null)
                    {
                        var validateMethod = genericType.GetMethod(nameof(IValidator.ValidateAsync));
                        if (validateMethod != null)
                        {
                            var validationResultTask = (Task<ValidationResult>?)validateMethod.Invoke(validator, new object[] { model, cancellation });
                            if (validationResultTask != null)
                            {
                                var validationResult = await validationResultTask;
                                if (!validationResult.IsValid)
                                {
                                    foreach (var error in validationResult.Errors)
                                    {
                                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                                    }
                                    flag = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        return flag;
    }
}