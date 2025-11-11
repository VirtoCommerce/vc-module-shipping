using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

public class PickupLocationIndexedSearchResult : PickupLocationSearchResult, IHasAggregations
{
    public virtual IList<Aggregation> Aggregations { get; set; }
}
