using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

namespace VirtoCommerce.ShippingModule.Core.Search.Indexed;

public interface IPickupLocationIndexedSearchService : ISearchService<PickupLocationIndexedSearchCriteria, PickupLocationIndexedSearchResult, PickupLocation>;
