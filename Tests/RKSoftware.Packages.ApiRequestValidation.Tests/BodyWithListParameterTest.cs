using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;
public class BodyWithListParameterTest
{
    [Fact]
    public async void TestRequestWhenBodyHasNonEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeListModel>>
        {
            new("model", BindingSource.Body, new FakeListModel
            {
                Items = 
                [
                    new FakeListItemModel
                    {
                        SystemName = "Test",
                    }
                ]
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async void TestRequestWhenBodyHasEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeListModel>>
        {
            new("model", BindingSource.Body, new FakeListModel
            {
                Items = null
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.TryGetValue(nameof(FakeListModel.Items), out ModelStateEntry? modelStateEntry));

        Assert.NotNull(modelStateEntry);

        Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

        Assert.Contains(FakeListModelValidator.ItemsErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
    }

    [Fact]
    public async void TestRequestWhenBodyHasEmptyRequiredChildProperty()
    {
        var paramaters = new List<ParameterModel<FakeListModel>>
        {
            new("model", BindingSource.Body, new FakeListModel
            {
                Items =
                [
                    new FakeListItemModel
                    {
                        SystemName = null,
                    }
                ]
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.TryGetValue("Items[0].SystemName", out ModelStateEntry? modelStateEntry));

        Assert.NotNull(modelStateEntry);

        Assert.True(modelStateEntry.ValidationState == ModelValidationState.Invalid);

        Assert.Contains(FakeListItemModelValidator.SystemNameErrorMessage, modelStateEntry.Errors.Select(x => x.ErrorMessage));
    }
}
