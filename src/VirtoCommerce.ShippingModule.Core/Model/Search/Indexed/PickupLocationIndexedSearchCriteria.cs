using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

public class PickupLocationIndexedSearchCriteria : PickupLocationSearchCriteria, IHasFacet
{
    public string Facet { get; set; }
    public string Filter { get; set; }
}
