using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ShippingModule.Core
{
    [ExcludeFromCodeCoverage]
    public class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "shipping:read";
                public const string Create = "shipping:create";
                public const string Update = "shipping:update";
                public const string Delete = "shipping:delete";

                public static readonly string[] AllPermissions = [Read, Create, Update, Delete];
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static IEnumerable<SettingDescriptor> AllSettings => Enumerable.Empty<SettingDescriptor>();
            }

            public static class FixedRateShippingMethod
            {
                public static SettingDescriptor GroundRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Shipping.FixedRateShippingMethod.Ground.Rate",
                    GroupName = "Shipping|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static SettingDescriptor AirRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Shipping.FixedRateShippingMethod.Air.Rate",
                    GroupName = "Shipping|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return GroundRate;
                        yield return AirRate;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings.Concat(FixedRateShippingMethod.AllSettings);
        }
    }
}
