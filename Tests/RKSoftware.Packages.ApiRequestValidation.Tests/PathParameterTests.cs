using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RKSoftware.Packages.ApiRequestValidation.Fakes;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class PathParameterTests
{
    private readonly IAsyncActionFilter _filter = new ApiRequestValidationAttribute();

    [Fact]
    public async Task TestRequestWithoutPathParameter()
    {
        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext<object>(null, _filter);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWithPathParameter()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new("id", BindingSource.Path, "test12345")
            
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWithTwoPathParameters()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new ParameterModel<string>("id", BindingSource.Path, "test_1"),
            new ParameterModel<string>("childId", BindingSource.Path, "test_2")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWithNullInPathParameter()
    {
        var paramaters = new List<ParameterModel<string?>>
        {
            new("id", BindingSource.Path, null)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWithNullInOneOfTwoPathParameters()
    {
        var paramaters = new List<ParameterModel<string?>>
        {
            new("id", BindingSource.Path, null),
            new("childId", BindingSource.Path, "test_2")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
    }  
}