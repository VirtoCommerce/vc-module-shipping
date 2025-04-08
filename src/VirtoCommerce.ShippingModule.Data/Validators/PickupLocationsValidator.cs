using System.Collections.Generic;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Validators;

public class PickupLocationsValidator : AbstractValidator<IEnumerable<PickupLocation>>
{
    public PickupLocationsValidator()
    {
        RuleFor(x => x).NotNull();
        var pickupLocationValidator = AbstractTypeFactory<PickupLocationValidator>.TryCreateInstance();
        RuleForEach(x => x).SetValidator(pickupLocationValidator);
    }
}
