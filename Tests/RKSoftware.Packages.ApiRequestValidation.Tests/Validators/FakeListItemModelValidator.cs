using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;
internal class FakeListItemModelValidator : AbstractValidator<FakeListItemModel>
{
    public const string SystemNameErrorMessage = "SystemName is required.";

    public FakeListItemModelValidator(CustomModel customModel)
    {
        RuleFor(x => x.SystemName)
            .NotEmpty()
            .WithMessage(SystemNameErrorMessage);
    }
}
