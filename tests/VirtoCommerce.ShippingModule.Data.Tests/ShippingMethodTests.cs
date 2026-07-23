using System;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.JsonConverters;
using VirtoCommerce.ShippingModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ShippingModule.Data.Tests;

public class ShippingMethodTests
{
    private readonly JsonSerializerSettings _serializerSettings;

    public ShippingMethodTests()
    {
        AbstractTypeFactory<ShippingRate>.OverrideType<ShippingRate, CustomShippingRate>();

        _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PolymorphJsonContractResolver(),
        };
    }

    [Fact]
    public void CalculateRates_ShouldReturnSerializableCustomShippingRate_WhenShippingRateOverridden()
    {
        // Arrange
        ShippingMethod[] methods = [
            new FixedRateShippingMethod { Settings = [] },
            new BuyOnlinePickupInStoreShippingMethod { Settings = [] }
        ];

        // Act
        var rates = methods
            .SelectMany(x => x.CalculateRates(new ShippingRateEvaluationContext { Currency = "USD" }))
            .ToArray();

        var json = JsonConvert.SerializeObject(rates, _serializerSettings);

        // Assert
        Assert.All(rates, rate => Assert.IsType<CustomShippingRate>(rate));
        Assert.Contains("Air", json);
        Assert.Contains("Ground", json);
        Assert.Contains("Pickup", json);
        Assert.Contains(nameof(CustomShippingRate.IsCustomRate), json, StringComparison.OrdinalIgnoreCase);
    }

    public class CustomShippingRate : ShippingRate
    {
        public bool IsCustomRate { get; set; }
    }
}
