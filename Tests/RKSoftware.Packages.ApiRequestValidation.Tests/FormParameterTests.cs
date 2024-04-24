using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class FormParameterTests
{
    [Fact]
    public async void TestRequestWhenFormIsNull()
    {
        FakeInputModel? query = null;
        var paramaters = new List<ParameterModel<FakeInputModel?>>
        {
            new ParameterModel<FakeInputModel?>("model", BindingSource.Form, query)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.First().Value?.Errors.First().ErrorMessage == "Form object is null.");
    }

    [Fact]
    public async void TestRequestWhenFormHasNonEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new ParameterModel<FakeInputModel>("model", BindingSource.Form, new FakeInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async void TestRequestWhenFormHasEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new ParameterModel<FakeInputModel>("model", BindingSource.Form, new FakeInputModel
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
    public async void TestRequestWhenFormIsSystemType()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new ParameterModel<string>("model", BindingSource.Form, "test_1234")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);
    }
}