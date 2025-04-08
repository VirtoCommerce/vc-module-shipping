using System.Collections.Generic;
using FluentValidation;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Validators;

public class PickupLocationsValidator : AbstractValidator<IEnumerable<PickupLocation>>
{
    public PickupLocationsValidator()
    {
        RuleFor(x => x).NotNull();
        RuleForEach(x => x).SetValidator(new PickupLocationValidator());
    }
}
