using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Fakes;

public class FakeGodInputModelValidator : FluentValidation.AbstractValidator<FakeGodInputModel>
{
    public const string SystemNameErrorMessage = "SystemName is required.";

    public FakeGodInputModelValidator(ICustomService customService)
    {
        customService.DoSomething();

        RuleFor(x => x.SystemName)
            .NotEmpty()
            .WithMessage(SystemNameErrorMessage);

        RuleFor(x => x.CustomProperty)
            .NotEmpty();

        RuleFor(x => x.CustomProperty1)
            .NotEmpty();

        RuleFor(x => x.CustomProperty2)
            .NotEmpty();

        RuleFor(x => x.CustomProperty3)
            .NotEmpty();

        RuleFor(x => x.CustomProperty4)
            .NotEmpty();

        RuleFor(x => x.CustomProperty5)
            .NotEmpty();

        RuleFor(x => x.CustomProperty6)
            .NotEmpty();

        RuleFor(x => x.CustomProperty7)
            .NotEmpty();

        RuleFor(x => x.CustomProperty8)
            .NotEmpty();

        RuleFor(x => x.CustomProperty9)
            .NotEmpty();

        RuleFor(x => x.CustomProperty10)
            .NotEmpty();

        RuleFor(x => x.CustomProperty11)
            .NotEmpty();

        RuleFor(x => x.CustomProperty12)
           .NotEmpty();

        RuleFor(x => x.CustomProperty13)
           .NotEmpty();

        RuleFor(x => x.CustomProperty14)
           .NotEmpty();


        RuleFor(x => x.CustomProperty15)
           .NotEmpty();
    }
}
