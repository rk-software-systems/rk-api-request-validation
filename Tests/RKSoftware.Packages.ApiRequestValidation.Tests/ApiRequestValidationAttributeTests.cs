using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace RKSoftware.Packages.ApiRequestValidation.Tests
{
    public class ApiRequestValidationAttributeTests
    {
        [Fact]
        public async void TestRequestWithoutPathParameter()
        {
            var actionExecutingContext = await GetActionExecutingContext(null);

            Assert.Null(actionExecutingContext.Result);
        }

        [Fact]
        public async void TestRequestWithPathParameter()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("id", BindingSource.Path, "test12345")
                
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);
        }

        [Fact]
        public async void TestRequestWithTwoPathParameters()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("id", BindingSource.Path, "test_1"),
                new ParameterModel("childId", BindingSource.Path, "test_2")
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);
        }

        [Fact]
        public async void TestRequestWithNullInPathParameter()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("id", BindingSource.Path, null)
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
        }

        [Fact]
        public async void TestRequestWithNullInOneOfTwoPathParameter()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("id", BindingSource.Path, null),
                new ParameterModel("childId", BindingSource.Path, "test_2")
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
        }

        private async Task<ActionExecutingContext> GetActionExecutingContext(List<ParameterModel>? parameters)
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
                Parameters = parameterDescriptors
            };

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                actionDescriptor);

            var actionArguments = parameters?
                .Select(x => new {x.Name, x.Value })
                .ToDictionary(x => x.Name, y => y.Value) ?? new Dictionary<string, object>();

            var controller = new FakeController();

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


        //public async void Common()
        //{
        //    var services = new ServiceCollection();
        //    services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        //    services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        //    services.AddMvcCore();
        //    services.AddOptions();
        //    var serviceProvider = services.BuildServiceProvider();

        //    var options = serviceProvider.GetService<IOptions<MvcOptions>>();
        //    var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(new List<IMetadataDetailsProvider>());
        //    var metadataProvider = new DefaultModelMetadataProvider(compositeDetailsProvider);
        //    var modelBinderFactory = new ModelBinderFactory(metadataProvider, options, serviceProvider);

        //    var parameterBinder = new ParameterBinder(
        //        metadataProvider,
        //        modelBinderFactory,
        //        new Mock<IObjectModelValidator>().Object,
        //        options,
        //        NullLoggerFactory.Instance);

        //    var httpContext = new DefaultHttpContext
        //    {
        //        RequestServices = serviceProvider // You must set this otherwise BinderTypeModelBinder will not resolve the specified type
        //    };

        //    var parameter = new ParameterDescriptor
        //    {
        //        Name = "id",
        //        BindingInfo = bindingInfo,
        //        ParameterType = typeof(string),
        //    };

        //    var modelMetadata = metadataProvider.GetMetadataForType(parameter.ParameterType);
        //    var routeDictionary = new RouteValueDictionary();
        //    routeDictionary.Add("id1", "test1234567");
        //    var valueProvider = new RouteValueProvider(BindingSource.Path, routeDictionary);
        //    var modelBinder = modelBinderFactory.CreateBinder(new ModelBinderFactoryContext()
        //    {
        //        BindingInfo = parameter.BindingInfo,
        //        Metadata = modelMetadata,
        //        CacheToken = parameter
        //    });

        //    var modelBindingResult = await parameterBinder.BindModelAsync(
        //        controllerContext,
        //        modelBinder,
        //        valueProvider,
        //        parameter,
        //        modelMetadata,
        //        value: null);

        //    var controller = new FakeController
        //    {
        //        ControllerContext = controllerContext
        //    };            
        //}

    }
}