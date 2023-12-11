using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data
{
    public class FixedRateShippingMethod : ShippingMethod
    {
        public FixedRateShippingMethod() : base("FixedRate")
        {
            Name = "Fixed shipping rate";
        }

        private decimal GroundOptionRate
            => Settings.GetValue<decimal>(ModuleConstants.Settings.FixedRateShippingMethod.GroundRate);

        private decimal AirOptionRate
            => Settings.GetValue<decimal>(ModuleConstants.Settings.FixedRateShippingMethod.AirRate);

        public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
        {
            if (!(context is ShippingRateEvaluationContext shippingContext))
            {
                throw new ArgumentException(nameof(context));
            }
            return new[]
            {
                new ShippingRate { Rate = GroundOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Ground" },
                new ShippingRate { Rate = AirOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Air" }
            };
        }
    }
}
