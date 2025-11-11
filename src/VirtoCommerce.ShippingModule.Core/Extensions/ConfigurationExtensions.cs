using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsPickupLocationFullTextSearchEnabled(this IConfiguration configuration)
    {
        var value = configuration["Search:PickupLocationFullTextSearchEnabled"];
        return value.TryParse(false);
    }
}
