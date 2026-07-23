using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data
{
    public class FixedRateShippingMethod : ShippingMethod
    {
        public FixedRateShippingMethod() : base(ModuleConstants.FixedRateShipmentCode)
        {
            Name = "Fixed shipping rate";
        }

        private decimal GroundOptionRate
            => Settings.GetValue<decimal>(ModuleConstants.Settings.FixedRateShippingMethod.GroundRate);

        private decimal AirOptionRate
            => Settings.GetValue<decimal>(ModuleConstants.Settings.FixedRateShippingMethod.AirRate);

        public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
        {
            if (context is not ShippingRateEvaluationContext shippingContext)
            {
                throw new ArgumentException($"Expected context of type {nameof(ShippingRateEvaluationContext)}.", nameof(context));
            }

            var ground = AbstractTypeFactory<ShippingRate>.TryCreateInstance();
            ground.Rate = GroundOptionRate;
            ground.Currency = shippingContext.Currency;
            ground.ShippingMethod = this;
            ground.OptionName = "Ground";

            var air = AbstractTypeFactory<ShippingRate>.TryCreateInstance();
            air.Rate = AirOptionRate;
            air.Currency = shippingContext.Currency;
            air.ShippingMethod = this;
            air.OptionName = "Air";

            return [ground, air];
        }
    }
}
