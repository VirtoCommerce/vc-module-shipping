using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Data.Model;

public class PickupFulfillmentRelationEntity : Entity
{
    [Required]
    [StringLength(128)]
    public string FulfillmentCenterId { get; set; }
    public string PickupLocationId { get; set; }
    public PickupLocationEntity PickupLocation { get; set; }

    public virtual void Patch(PickupFulfillmentRelationEntity target)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
    }
}
