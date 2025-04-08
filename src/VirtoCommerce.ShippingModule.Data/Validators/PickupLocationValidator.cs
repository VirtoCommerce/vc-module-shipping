using FluentValidation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Validators;

public class PickupLocationValidator : AbstractValidator<PickupLocation>
{
    public PickupLocationValidator()
    {
        RuleFor(x => x.Name).MaximumLength(2048);
        RuleFor(x => x.Address).NotNull().When(x => x.IsActive);
        var addressValidator = AbstractTypeFactory<PickupLocationAddressValidator>.TryCreateInstance();
        RuleFor(x => x.Address).SetValidator(addressValidator).When(x => x.IsActive);
    }
}
