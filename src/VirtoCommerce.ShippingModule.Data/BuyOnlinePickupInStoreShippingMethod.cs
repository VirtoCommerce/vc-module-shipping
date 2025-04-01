using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data;

public class BuyOnlinePickupInStoreShippingMethod : ShippingMethod
{
    public BuyOnlinePickupInStoreShippingMethod() : base("BuyOnlinePickupInStore")
    {
        Name = "Buy Online Pickup In Store";
    }

    public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
    {
        if (context is not ShippingRateEvaluationContext shippingContext)
        {
            throw new ArgumentException($"Expected context of type {nameof(ShippingRateEvaluationContext)}.", nameof(context));
        }

        return [
            new ShippingRate
            {
                Rate = 0,
                Currency = shippingContext.Currency,
                ShippingMethod = this,
                OptionName = "Pickup",
            },
        ];
    }
}
