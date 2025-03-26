using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Model;

public class PickupLocationEntity : AuditableEntity, IDataEntity<PickupLocationEntity, PickupLocation>, IHasOuterId
{
    [StringLength(128)]
    public string StoreId { get; set; }
    public bool Active { get; set; }
    [StringLength(1024)]
    public string Description { get; set; }

    [StringLength(128)]
    public string FulfillmentCenterId { get; set; }

    public virtual ObservableCollection<PickupFulfillmentRelationEntity> TransferFulfillmentCenters { get; set; }
        = new NullCollection<PickupFulfillmentRelationEntity>();

    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string WorkingHours { get; set; }

    [StringLength(128)]
    public string Name { get; set; }
    [Required]
    [StringLength(1024)]
    public string Line1 { get; set; }

    [StringLength(1024)]
    public string Line2 { get; set; }

    [Required]
    [StringLength(128)]
    public string City { get; set; }

    [Required]
    [StringLength(64)]
    public string CountryCode { get; set; }

    [StringLength(128)]
    public string CountryName { get; set; }

    [Required]
    [StringLength(32)]
    public string PostalCode { get; set; }

    [StringLength(128)]
    public string RegionId { get; set; }

    [StringLength(128)]
    public string RegionName { get; set; }

    [StringLength(64)]
    public string GeoLocation { get; set; }

    [StringLength(128)]
    public string OuterId { get; set; }

    public PickupLocation ToModel(PickupLocation model)
    {
        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;
        model.Name = Name;
        model.StoreId = StoreId;
        model.OuterId = OuterId;
        model.Active = Active;
        model.GeoLocation = GeoLocation;
        model.Description = Description;
        model.FulfillmentCenterId = FulfillmentCenterId;
        model.TransferFulfillmentCenterIds = TransferFulfillmentCenters.Select(x => x.FulfillmentCenterId).ToList();
        model.ContactEmail = ContactEmail;
        model.ContactPhone = ContactPhone;
        model.WorkingHours = WorkingHours;

        model.Address = AbstractTypeFactory<Address>.TryCreateInstance();

        model.Address.Id = Id;
        model.Address.Name = Name;
        model.Address.Line1 = Line1;
        model.Address.Line2 = Line2;
        model.Address.City = City;
        model.Address.CountryCode = CountryCode;
        model.Address.CountryName = CountryName;
        model.Address.PostalCode = PostalCode;
        model.Address.RegionId = RegionId;
        model.Address.RegionName = RegionName;

        return model;
    }

    public PickupLocationEntity FromModel(PickupLocation model, PrimaryKeyResolvingMap pkMap)
    {
        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;
        StoreId = model.StoreId;
        OuterId = model.OuterId;
        Active = model.Active;
        GeoLocation = model.GeoLocation;
        Name = model.Name; // this name is primary (not from address)
        Description = model.Description;
        FulfillmentCenterId = model.FulfillmentCenterId;
        ContactEmail = model.ContactEmail;
        ContactPhone = model.ContactPhone;
        WorkingHours = model.WorkingHours;

        if (model.TransferFulfillmentCenterIds != null)
        {
            TransferFulfillmentCenters.AddRange(model.TransferFulfillmentCenterIds.Where(x => x != null).Select(x => new PickupFulfillmentRelationEntity
            {
                FulfillmentCenterId = x,
                PickupLocationId = model.Id,
            }));

        }

        if (model.Address != null)
        {
            Line1 = model.Address.Line1;
            Line2 = model.Address.Line2;
            City = model.Address.City;
            CountryCode = model.Address.CountryCode;
            CountryName = model.Address.CountryName;
            PostalCode = model.Address.PostalCode;
            RegionId = model.Address.RegionId;
            RegionName = model.Address.RegionName;
        }

        return this;
    }

    public void Patch(PickupLocationEntity target)
    {
        target.CreatedBy = CreatedBy;
        target.CreatedDate = CreatedDate;
        target.ModifiedBy = ModifiedBy;
        target.ModifiedDate = ModifiedDate;
        target.StoreId = StoreId;
        target.OuterId = OuterId;
        target.Active = Active;
        target.GeoLocation = GeoLocation;
        target.Description = Description;
        target.FulfillmentCenterId = FulfillmentCenterId;
        target.ContactEmail = ContactEmail;
        target.ContactPhone = ContactPhone;
        target.WorkingHours = WorkingHours;
        target.Name = Name;
        target.Line1 = Line1;
        target.Line2 = Line2;
        target.City = City;
        target.CountryCode = CountryCode;
        target.CountryName = CountryName;
        target.PostalCode = PostalCode;
        target.RegionId = RegionId;
        target.RegionName = RegionName;

        if (!TransferFulfillmentCenters.IsNullCollection())
        {
            var fulfillmentCenterComparer = AnonymousComparer.Create((PickupFulfillmentRelationEntity fc) => $"{fc.FulfillmentCenterId}");
            TransferFulfillmentCenters.Patch(target.TransferFulfillmentCenters, fulfillmentCenterComparer,
                (sourceFulfillmentCenter, targetFulfillmentCenter) => sourceFulfillmentCenter.Patch(targetFulfillmentCenter));
        }
    }
}
