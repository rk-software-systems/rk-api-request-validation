using FluentValidation;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;
internal class FakeListModelValidator : AbstractValidator<FakeListModel>
{
    public const string ItemsErrorMessage = "Items is required.";

    public FakeListModelValidator()
    {
        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage(ItemsErrorMessage);

        var customModel = new CustomModel();

        RuleForEach(x => x.Items).SetValidator(new FakeListItemModelValidator(customModel));
    }
}
