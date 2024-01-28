using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

internal class FakeInputModelValidator : AbstractValidator<FakeInputModel>
{
    public const string SystemNameErrorMessage = "SystemName is required.";

    public FakeInputModelValidator()
    {
        RuleFor(x => x.SystemName)
            .NotEmpty()
            .WithMessage(SystemNameErrorMessage);
    }
}
