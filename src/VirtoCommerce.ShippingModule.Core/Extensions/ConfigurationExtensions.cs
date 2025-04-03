using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsPickupEnabled(this IConfiguration configuration)
    {
        var value = configuration["Shipping:IsPickupEnabled"];
        return value.TryParse(false);
    }
}
