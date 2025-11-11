using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Model;

public class PickupLocation : AuditableEntity, ICloneable, IHasRelevanceScore
{
    [StringLength(128)]
    public string StoreId { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }

    [StringLength(1024)]
    public string Description { get; set; }

    [StringLength(128)]
    public string FulfillmentCenterId { get; set; }

    public ICollection<string> TransferFulfillmentCenterIds { get; set; }

    public string ContactPhone { get; set; }

    public string ContactEmail { get; set; }

    public int? DeliveryDays { get; set; }
    public int? StorageDays { get; set; }

    public string WorkingHours { get; set; }

    public string GeoLocation { get; set; }

    public PickupLocationAddress Address { get; set; }

    public string OuterId { get; set; }

    public double? RelevanceScore { get; set; }

    public object Clone()
    {
        var result = (PickupLocation)MemberwiseClone();

        if (Address != null)
        {
            result.Address = Address.CloneTyped();
        }

        return result;
    }
}
