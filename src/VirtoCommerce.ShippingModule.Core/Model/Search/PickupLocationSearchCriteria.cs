using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model.Search;

public class PickupLocationSearchCriteria : SearchCriteriaBase
{
    public bool? IsActive { get; set; }
    public string StoreId { get; set; }

    //TODO: address fields here or in PickupLocationIndexedSearchCriteria only?
}
