using FluentValidation;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Validators;

public class PickupLocationValidator : AbstractValidator<PickupLocation>
{
    public PickupLocationValidator()
    {
        RuleFor(x => x.Name).MaximumLength(2048);
        RuleFor(x => x.Address).NotNull().When(x => x.IsActive);
        RuleFor(x => x.Address).SetValidator(new PickupLocationAddressValidator()).When(x => x.IsActive);
    }
}
