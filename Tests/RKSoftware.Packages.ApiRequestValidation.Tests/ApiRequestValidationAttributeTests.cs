using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RKSoftware.Packages.ApiRequestValidation.Tests
{
    public class ApiRequestValidationAttributeTests
    {
        #region test path parameter

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
        public async void TestRequestWithNullInOneOfTwoPathParameters()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("id", BindingSource.Path, null),
                new ParameterModel("childId", BindingSource.Path, "test_2")
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
        }
        #endregion

        #region test body parameter

        [Fact]
        public async void TestRequestWhenBodyIsNull()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Body, null)
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

            Assert.True(actionExecutingContext.ModelState.IsValid);

            Assert.True(actionExecutingContext.ModelState.First().Value.Errors.First().ErrorMessage == "Body is null.");
        }

        [Fact]
        public async void TestRequestWhenBodyHasNonEmptyRequiredProperty()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Body, new FakeInputModel
                {
                    SystemName = "test_1"
                })
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);

            Assert.True(actionExecutingContext.ModelState.IsValid);
        }

        [Fact]
        public async void TestRequestWhenBodyHasEmptyRequiredProperty()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Body, new FakeInputModel
                {
                    SystemName = null
                })
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

            Assert.False(actionExecutingContext.ModelState.IsValid);

            Assert.True(actionExecutingContext.ModelState.TryGetValue(nameof(FakeInputModel.SystemName), out ModelStateEntry modelStateEntry));

            Assert.NotNull(modelStateEntry);

            Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

            Assert.Contains(FakeInputModelValidator.SystemNameErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
        }

        [Fact]
        public async void TestRequestWhenBodyIsSystemType()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Body, "test_1234")
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);
        }
        #endregion

        #region test form parameter

        [Fact]
        public async void TestRequestWhenFormIsNull()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Form, null)
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

            Assert.False(actionExecutingContext.ModelState.IsValid);

            Assert.True(actionExecutingContext.ModelState.First().Value.Errors.First().ErrorMessage == "Form is null.");
        }

        [Fact]
        public async void TestRequestWhenFormHasNonEmptyRequiredProperty()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Form, new FakeInputModel
                {
                    SystemName = "test_1"
                })
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);

            Assert.True(actionExecutingContext.ModelState.IsValid);
        }

        [Fact]
        public async void TestRequestWhenFormHasEmptyRequiredProperty()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Form, new FakeInputModel
                {
                    SystemName = null
                })
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

            Assert.False(actionExecutingContext.ModelState.IsValid);

            Assert.True(actionExecutingContext.ModelState.TryGetValue(nameof(FakeInputModel.SystemName), out ModelStateEntry modelStateEntry));

            Assert.NotNull(modelStateEntry);

            Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

            Assert.Contains(FakeInputModelValidator.SystemNameErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
        }

        [Fact]
        public async void TestRequestWhenFormIsSystemType()
        {
            var paramaters = new List<ParameterModel>
            {
                new ParameterModel("model", BindingSource.Form, "test_1234")
            };

            var actionExecutingContext = await GetActionExecutingContext(paramaters);

            Assert.Null(actionExecutingContext.Result);
        }
        #endregion

        #region helpers
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
                GetDefaultHttpContext(),
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

        private DefaultHttpContext GetDefaultHttpContext()
        {
            var services = new ServiceCollection();
            services.AddScoped<IValidator<FakeInputModel>, FakeInputModelValidator>();
            var serviceProvider = services.BuildServiceProvider();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            return httpContext;
        }

        #endregion
    }
}