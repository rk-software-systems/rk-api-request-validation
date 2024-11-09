using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class QueryParameterTests
{
    [Fact]
    public async Task TestRequestWhenQueryIsNull()
    {
        ParameterModel? query = null;
        var paramaters = new List<ParameterModel<ParameterModel?>>
        {
            new("model", BindingSource.Query, query)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        Assert.False(actionExecutingContext.ModelState.IsValid);

        Assert.True(actionExecutingContext.ModelState.First().Value?.Errors.First().ErrorMessage == "Query object is null.");
    }

    [Fact]
    public async Task TestRequestWhenQueryIsSystemTypeAndNull()
    {
        int? query = null;
        var paramaters = new List<ParameterModel<int?>>
        {
            new("model", BindingSource.Query, query)
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.True(actionExecutingContext.ModelState.IsValid);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWhenQueryHasNonEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new("model", BindingSource.Query, new FakeInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async Task TestRequestWhenQueryHasEmptyRequiredProperty()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new("model", BindingSource.Query, new FakeInputModel
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
    public async Task TestRequestWhenQueryIsSystemType()
    {
        var paramaters = new List<ParameterModel<string>>
        {
            new("model", BindingSource.Query, "test_1234")
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    public async Task TestRequestWhenQueryHasNonEmptyRequiredPropertyTogetherWithValidBody()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
             new("modelBody", BindingSource.Body, new FakeInputModel
            {
                SystemName = "test_body_1"
            }),
            new("model", BindingSource.Query, new FakeInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async Task TestRequestWhenQueryHasEmptyRequiredPropertyTogetherWithValidBody()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new("modelBody", BindingSource.Body, new FakeInputModel
            {
                SystemName = "test_body_1"
            }),
            new("model", BindingSource.Query, new FakeInputModel
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
    public async Task TestRequestWhenQueryHasNonEmptyRequiredPropertyTogetherWithValidForm()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
             new("modelForm", BindingSource.Form, new FakeInputModel
            {
                SystemName = "test_form_1"
            }),
            new("model", BindingSource.Query, new FakeInputModel
            {
                SystemName = "test_1"
            })
        };

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters);

        Assert.Null(actionExecutingContext.Result);

        Assert.True(actionExecutingContext.ModelState.IsValid);
    }

    [Fact]
    public async Task TestRequestWhenQueryHasEmptyRequiredPropertyTogetherWithValidForm()
    {
        var paramaters = new List<ParameterModel<FakeInputModel>>
        {
            new("modelForm", BindingSource.Body, new FakeInputModel
            {
                SystemName = "test_form_1"
            }),
            new("model", BindingSource.Query, new FakeInputModel
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
}