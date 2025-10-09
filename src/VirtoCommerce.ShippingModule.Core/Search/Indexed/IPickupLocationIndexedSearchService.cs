using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

namespace VirtoCommerce.ShippingModule.Core.Search.Indexed;

public interface IPickupLocationIndexedSearchService
{
    Task<PickupLocationIndexedSearchResult> SearchPickupLocationsAsync(PickupLocationIndexedSearchCriteria searchCriteria);
}
