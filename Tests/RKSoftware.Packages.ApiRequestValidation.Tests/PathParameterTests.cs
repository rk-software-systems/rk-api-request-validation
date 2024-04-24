using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class PathParameterTests
{
    [Fact]
    public async void TestRequestWithoutPathParameter()
    {
        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext<object>(null);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async void TestRequestWithPathParameter()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new("id", BindingSource.Path, "test12345")
            
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async void TestRequestWithTwoPathParameters()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new ParameterModel<string>("id", BindingSource.Path, "test_1"),
            new ParameterModel<string>("childId", BindingSource.Path, "test_2")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async void TestRequestWithNullInPathParameter()
    {
        var paramaters = new List<ParameterModel<string?>>
        {
            new("id", BindingSource.Path, null)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
    }

    [Fact]
    public async void TestRequestWithNullInOneOfTwoPathParameters()
    {
        var paramaters = new List<ParameterModel<string?>>
        {
            new("id", BindingSource.Path, null),
            new("childId", BindingSource.Path, "test_2")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<NotFoundResult>(actionExecutingContext.Result);
    }  
}