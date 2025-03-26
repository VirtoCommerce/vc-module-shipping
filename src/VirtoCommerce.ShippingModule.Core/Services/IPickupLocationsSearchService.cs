using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;

namespace VirtoCommerce.ShippingModule.Core.Services;

public interface IPickupLocationsSearchService
    : ISearchService<PickupLocationsSearchCriteria, PickupLocationsSearchResult, PickupLocation>;
