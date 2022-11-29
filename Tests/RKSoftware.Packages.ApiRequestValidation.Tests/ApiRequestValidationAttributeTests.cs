using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace RKSoftware.Packages.ApiRequestValidation.Tests
{
    public class ApiRequestValidationAttributeTests
    {
        [Fact]
        public async void TestRequestWithNullInPathParameter()
        {

            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            var metadata = new List<IFilterMetadata>();

            var result = new Dictionary<string, object>();

            var controller = Mock.Of<Controller>();

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                metadata,
                result,
                controller
            );

            ActionExecutionDelegate next = () => {
                var ctx = new ActionExecutedContext(actionContext, metadata, controller);
                return Task.FromResult(ctx);
            };

            var attribute = new ApiRequestValidationAttribute();

            await attribute.OnActionExecutionAsync(actionExecutingContext, next);
        }
    }
}