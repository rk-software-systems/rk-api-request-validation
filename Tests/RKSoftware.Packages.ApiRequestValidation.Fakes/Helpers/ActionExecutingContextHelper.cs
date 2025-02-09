using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Fakes;

public static class ActionExecutingContextHelper
{
    public static async Task<ActionExecutingContext> GetActionExecutingContext<T>(
        List<ParameterModel<T>>? parameters,
        IAsyncActionFilter apiRequestValidationAttribute)
    {

        ArgumentNullException.ThrowIfNull(apiRequestValidationAttribute, nameof(apiRequestValidationAttribute));

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

        var actionArguments = parameters?
            .Select(x => new { x.Name, x.Value })
            .ToDictionary(x => x.Name, y => y.Value) ?? [];

        var actionContext = new ActionContext(
            GetDefaultHttpContext(),
            new RouteData(),
            actionDescriptor);

        var controller = new FakeController
        {
            ControllerContext = new ControllerContext(actionContext)
        };

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            [],
            actionArguments,
            controller
        );

        Task<ActionExecutedContext> next()
        {
            var ctx = new ActionExecutedContext(
                actionContext,
                [],
                controller);

            return Task.FromResult(ctx);
        }

        await apiRequestValidationAttribute.OnActionExecutionAsync(actionExecutingContext, next);

        return actionExecutingContext;
    }

    public static async Task<ActionExecutingContext> GetActionExecutingContextSg<T>(
        List<ParameterModel<T>>? parameters,
        IAsyncActionFilter apiRequestValidationAttribute)
    {

        ArgumentNullException.ThrowIfNull(apiRequestValidationAttribute, nameof(apiRequestValidationAttribute));

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

        var actionArguments = parameters?
            .Select(x => new { x.Name, x.Value })
            .ToDictionary(x => x.Name, y => y.Value) ?? [];

        var actionContext = new ActionContext(
            GetDefaultHttpContextSg(),
            new RouteData(),
            actionDescriptor);

        var controller = new FakeController
        {
            ControllerContext = new ControllerContext(actionContext)
        };

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            [],
            actionArguments,
            controller
        );

        Task<ActionExecutedContext> next()
        {
            var ctx = new ActionExecutedContext(
                actionContext,
                [],
                controller);

            return Task.FromResult(ctx);
        }

        await apiRequestValidationAttribute.OnActionExecutionAsync(actionExecutingContext, next);

        return actionExecutingContext;
    }

    private static DefaultHttpContext GetDefaultHttpContext()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblyContaining<FakeInputModelValidator>();

        services.AddScoped<ICustomService, CustomService>();
        services.AddMvcCore();
        var serviceProvider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        return httpContext;
    }

    private static DefaultHttpContext GetDefaultHttpContextSg()
    {
        var services = new ServiceCollection();

        services.AddValidators();

        services.AddScoped<ICustomService, CustomService>();
        services.AddScoped<IValidationProcessor, ValidationProcessor>();
        services.AddMvcCore();
        var serviceProvider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        return httpContext;
    }
}
