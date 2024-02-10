using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class BodyParameterTests
{
    [Fact]
    public async void TestRequestWhenBodyIsNull()
    {
        var paramaters = new List<ParameterModel>
        {
            new("model", BindingSource.Body, null)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.First().Value?.Errors.First().ErrorMessage == "Body is null.");
    }

    [Fact]
    public async void TestRequestWhenBodyHasNonEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel>
        {
            new("model", BindingSource.Body, new FakeInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async void TestRequestWhenBodyHasEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel>
        {
            new("model", BindingSource.Body, new FakeInputModel
            {
                SystemName = null
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.TryGetValue(nameof(FakeInputModel.SystemName), out ModelStateEntry? modelStateEntry));

        Assert.NotNull(modelStateEntry);

        Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

        Assert.Contains(FakeInputModelValidator.SystemNameErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public async void TestRequestWhenBodyIsSystemType()
    {
        var paramaters = new List<ParameterModel>
        {
            new("model", BindingSource.Body, "test_1234")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);
    }
}