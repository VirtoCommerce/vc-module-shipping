using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data;

public class BuyOnlinePickupInStoreShippingMethod : ShippingMethod
{
    public BuyOnlinePickupInStoreShippingMethod() : base(ModuleConstants.BuyOnlinePickupInStoreShipmentCode)
    {
        Name = "Buy Online Pickup In Store";
    }

    public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
    {
        if (context is not ShippingRateEvaluationContext shippingContext)
        {
            throw new ArgumentException($"Expected context of type {nameof(ShippingRateEvaluationContext)}.", nameof(context));
        }

        var pickup = AbstractTypeFactory<ShippingRate>.TryCreateInstance();
        pickup.Rate = 0;
        pickup.Currency = shippingContext.Currency;
        pickup.ShippingMethod = this;
        pickup.OptionName = "Pickup";

        return [pickup];
    }
}
