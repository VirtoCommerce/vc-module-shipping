using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model;

public class PickupLocation : AuditableEntity, ICloneable
{
    [StringLength(128)]
    public string StoreId { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    [StringLength(1024)]
    public string Description { get; set; }

    [StringLength(128)]
    public string FulfillmentCenterId { get; set; }
    public ICollection<string> TransferFulfillmentCenterIds { get; set; }

    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string WorkingHours { get; set; }

    public string GeoLocation { get; set; }
    public PickupLocationAddress Address { get; set; }

    public string OuterId { get; set; }

    public object Clone()
    {
        var result = (PickupLocation)MemberwiseClone();

        if (Address != null)
        {
            result.Address = (PickupLocationAddress)Address.Clone();
        }

        return result;
    }
}
