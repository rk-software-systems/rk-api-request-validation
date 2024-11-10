using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Fakes;

public class FakeInputModelValidator : FluentValidation.AbstractValidator<FakeInputModel>
{
    public const string SystemNameErrorMessage = "SystemName is required.";

    public FakeInputModelValidator(ICustomService customService)
    {
        customService.DoSomething();

        RuleFor(x => x.SystemName)
            .NotEmpty()
            .WithMessage(SystemNameErrorMessage);        
    }
}
