using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RKSoftware.Packages.ApiRequestValidation.Fakes;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class BodyParameter2Tests
{
    private readonly IAsyncActionFilter _filter = new ApiRequestValidation2Attribute();

    [Fact]
    public async Task TestRequestWhenBodyIsNull()
    {
        FakeGodInputModel? body = null;
        var paramaters = new List<ParameterModel<FakeGodInputModel?>>
        {
            new("model", BindingSource.Body, body)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.First().Value?.Errors.First().ErrorMessage == "Body object is null.");
    }

    [Fact]
    public async Task TestRequestWhenBodyHasNonEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeGodInputModel>>
        {
            new("model", BindingSource.Body, new FakeGodInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async Task TestRequestWhenBodyHasEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeGodInputModel>>
        {
            new("model", BindingSource.Body, new FakeGodInputModel
            {
                SystemName = null
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.TryGetValue(nameof(FakeInputModel.SystemName), out ModelStateEntry? modelStateEntry));

        Assert.NotNull(modelStateEntry);

        Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

        Assert.Contains(FakeInputModelValidator.SystemNameErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public async Task TestRequestWhenBodyIsSystemType()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new("model", BindingSource.Body, "test_1234")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, _filter);

        Assert.Null(actionExecutingContext.Result);
    }
}