using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace RKSoftware.Packages.ApiRequestValidation.Fakes;

public class ValidationProcessor(IServiceProvider serviceProvider) : IValidationProcessor
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<ValidationResult?> Validate(string classNameToValidate, object obj)
    {
        switch (classNameToValidate)
        {
            case "RKSoftware.Packages.ApiRequestValidation.Fakes.FakeInputModel":
                var val1 = _serviceProvider.GetService<IValidator<FakeInputModel>>();
                if(val1 == null)
                {
                    return null;
                }
                var m1 = (FakeInputModel)obj;
                return await val1.ValidateAsync(m1);
            case "RKSoftware.Packages.ApiRequestValidation.Fakes.FakeGodInputModel":
                var val2 = _serviceProvider.GetService<IValidator<FakeGodInputModel>>();
                if (val2 == null)
                {
                    return null;
                }
                var m2 = (FakeGodInputModel)obj;
                return await val2.ValidateAsync(m2);
            case "RKSoftware.Packages.ApiRequestValidation.Fakes.FakeGodInputStructModel":
                var val3 = _serviceProvider.GetService<IValidator<FakeGodInputStructModel>>();
                if (val3 == null)
                {
                    return null;
                }
                var m3 = (FakeGodInputStructModel)obj;
                return await val3.ValidateAsync(m3);
            default:
                return null;
        }
    }
}
