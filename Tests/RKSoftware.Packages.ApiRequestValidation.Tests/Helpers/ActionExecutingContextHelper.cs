using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

internal class ActionExecutingContextHelper
{
    internal static async Task<ActionExecutingContext> GetActionExecutingContext(List<ParameterModel>? parameters)
    {

        var parameterDescriptors = parameters?.Select(x => new ParameterDescriptor
        {
            Name = x.Name,
            BindingInfo = new BindingInfo
            {
                BindingSource = x.BindingSource
            },
            ParameterType = x.Type,
        }).ToList();

        var actionDescriptor = new ControllerActionDescriptor
        {
            Parameters = parameterDescriptors ?? []
        };

        var actionContext = new ActionContext(
            GetDefaultHttpContext(),
            new RouteData(),
            actionDescriptor);

        var actionArguments = parameters?
            .Select(x => new { x.Name, x.Value })
            .ToDictionary(x => x.Name, y => y.Value) ?? [];

        var controller = new FakeController();
        controller.ControllerContext = new ControllerContext(actionContext);

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            actionArguments,
            controller
        );

        Task<ActionExecutedContext> next()
        {
            var ctx = new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller);

            return Task.FromResult(ctx);
        }

        var attribute = new ApiRequestValidationAttribute();

        await attribute.OnActionExecutionAsync(actionExecutingContext, next);

        return actionExecutingContext;
    }

    private static DefaultHttpContext GetDefaultHttpContext()
    {
        var services = new ServiceCollection();
        services.AddScoped<IValidator<FakeInputModel>, FakeInputModelValidator>();
        services.AddScoped<IValidator<FakeListModel>, FakeListModelValidator>();
        services.AddScoped<IValidator<FakeListItemModel>, FakeListItemModelValidator>();
        services.AddScoped<ICustomService, CustomService>();
        services.AddMvcCore();
        var serviceProvider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        return httpContext;
    }
}
