using System;
using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model.Search;

namespace VirtoCommerce.ShippingModule.Core.Services
{
    /// <summary>
    /// This interface should implement <see cref="SearchService<ShippingMethod>"/> without methods.
    /// Methods left for compatibility and should be removed after upgrade to inheritance
    /// </summary>
    public interface IShippingMethodsSearchService
    {
        [Obsolete(@"Need to remove after inherit IShippingMethodsSearchService from SearchService<ShippingMethod>.")]
        Task<ShippingMethodsSearchResult> SearchShippingMethodsAsync(ShippingMethodsSearchCriteria criteria);
    }
}
