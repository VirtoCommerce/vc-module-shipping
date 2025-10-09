using System.Collections.Generic;

namespace VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

public class PickupLocationIndexedSearchResult : PickupLocationSearchResult
{
    public virtual IList<PickupLocationAggregation> Aggregations { get; set; }
}
