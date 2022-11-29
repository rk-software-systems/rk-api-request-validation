using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation
{
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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameters = context
                 .ActionDescriptor
                 .Parameters
                 .Where(x => x.BindingInfo.BindingSource == BindingSource.Path)
                 .ToList();

            var flag = true;
            foreach (var parameter in parameters)
            {
                if (!context.ActionArguments.TryGetValue(parameter.Name, out object value) ||
                    value == null)
                {
                    flag = false;
                    break;
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
        public static async Task<bool> IsBodyOrFormParameterValid(this ActionExecutingContext context, CancellationToken cancellation = new CancellationToken())
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = context
                .ActionDescriptor
                .Parameters
                .FirstOrDefault(x => x.BindingInfo.BindingSource == BindingSource.Body ||
                                     x.BindingInfo.BindingSource == BindingSource.Form);
            var flag = true;
            if (parameter != null)
            {
                if (!context.ActionArguments.TryGetValue(parameter.Name, out object model) ||
                    model == null)
                {
                    context.ModelState.AddModelError("", $"{parameter.BindingInfo.BindingSource.Id} is null.");
                    flag = false;
                }
                else if (!(model.GetType().FullName?.StartsWith($"{nameof(System)}.", StringComparison.Ordinal)).GetValueOrDefault())
                {
                    var validatorType = typeof(IValidator<>);
                    var genericType = validatorType.MakeGenericType(model.GetType());

                    var validator = context.HttpContext.RequestServices.GetService(genericType);
                    if (validator != null)
                    {
                        var validateMethod = genericType.GetMethod(nameof(IValidator.ValidateAsync));
                        if (validateMethod != null)
                        {
                            var validationResultTask = (Task<ValidationResult>)validateMethod.Invoke(validator, new object[] { model, cancellation });
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
            return flag;
        }
    }
}