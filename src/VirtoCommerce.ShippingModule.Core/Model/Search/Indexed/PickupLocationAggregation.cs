using System.Collections.Generic;

namespace VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

public class PickupLocationAggregation
{
    /// <summary>
    /// Gets or sets the value of the aggregation type
    /// </summary>
    /// <value>
    /// "Attribute", "Range"
    /// </value>
    public string AggregationType { get; set; }

    /// <summary>
    /// Gets or sets the value of the aggregation field
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the collection of the aggregation labels
    /// </summary>
    public IList<PickupLocationAggregationLabel> Labels { get; set; }

    /// <summary>
    /// Gets or sets the collection of the aggregation items
    /// </summary>
    public IList<PickupLocationAggregationItem> Items { get; set; }
}
