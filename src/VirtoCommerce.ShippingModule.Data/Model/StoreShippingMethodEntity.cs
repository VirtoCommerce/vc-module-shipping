using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Model
{
    public class StoreShippingMethodEntity : Entity, IDataEntity<StoreShippingMethodEntity, ShippingMethod>
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        public bool IsActive { get; set; }

        public string TypeName { get; set; }

        public string StoreId { get; set; }

        public virtual ShippingMethod ToModel(ShippingMethod model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = Id;
            model.IsActive = IsActive;
            model.Code = Code;
            model.TaxType = TaxType;
            model.LogoUrl = LogoUrl;
            model.Priority = Priority;
            model.StoreId = StoreId;

            return model;
        }

        public virtual StoreShippingMethodEntity FromModel(ShippingMethod model, PrimaryKeyResolvingMap pkMap)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            pkMap.AddPair(model, this);

            Id = model.Id;
            IsActive = model.IsActive;
            Code = model.Code;
            TaxType = model.TaxType;
            LogoUrl = model.LogoUrl;
            Priority = model.Priority;
            StoreId = model.StoreId;
            TypeName = model.TypeName;

            return this;
        }

        public virtual void Patch(StoreShippingMethodEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.TaxType = TaxType;
            target.LogoUrl = LogoUrl;
            target.Priority = Priority;
            target.StoreId = StoreId;
        }
    }
}
