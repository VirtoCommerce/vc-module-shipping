using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationDocumentBuilder(IPickupLocationService pickupLocationService)
    : IIndexSchemaBuilder, IIndexDocumentBuilder
{
    public Task BuildSchemaAsync(IndexDocument schema)
    {
        schema.AddFilterableString(PickupLocationIndexFields.StoreId);
        schema.AddFilterableString(PickupLocationIndexFields.OuterId);

        schema.AddFilterableBoolean(PickupLocationIndexFields.IsActive);

        schema.AddFilterableValue(PickupLocationIndexFields.GeoLocation, null, IndexDocumentFieldValueType.GeoPoint);

        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.Name);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.Description);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.WorkingHours);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.ContactEmail);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.ContactPhone);

        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCountryCode);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCountryName);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressRegionId);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressRegionName);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCity);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressLine1);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressLine2);
        schema.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressPostalCode);

        return Task.CompletedTask;
    }

    public async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
    {
        var result = new List<IndexDocument>();

        var pickupLocations = await pickupLocationService.GetNoCloneAsync(documentIds);

        foreach (var pickupLocation in pickupLocations)
        {
            result.Add(await CreateDocument(pickupLocation));
        }

        return result;
    }

    protected virtual Task<IndexDocument> CreateDocument(PickupLocation pickupLocation)
    {
        var document = new IndexDocument(pickupLocation.Id);

        document.AddFilterableString(PickupLocationIndexFields.StoreId, pickupLocation.StoreId);
        document.AddFilterableString(PickupLocationIndexFields.OuterId, pickupLocation.OuterId);

        document.AddFilterableBoolean(PickupLocationIndexFields.IsActive, pickupLocation.IsActive);

        if (!pickupLocation.GeoLocation.IsNullOrEmpty())
        {
            document.AddFilterableValue(PickupLocationIndexFields.GeoLocation, GeoPoint.TryParse(pickupLocation.GeoLocation), IndexDocumentFieldValueType.GeoPoint);
        }

        document.AddFilterableStringAndContentString(PickupLocationIndexFields.Name, pickupLocation.Name);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.Description, pickupLocation.Description);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.WorkingHours, pickupLocation.WorkingHours);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.ContactEmail, pickupLocation.ContactEmail);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.ContactPhone, pickupLocation.ContactPhone);

        if (pickupLocation.Address != null)
        {
            IndexAddress(pickupLocation.Address, document);
        }

        return Task.FromResult(document);
    }

    protected virtual void IndexAddress(PickupLocationAddress address, IndexDocument document)
    {
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCountryCode, address.CountryCode);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCountryName, address.CountryName);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressRegionId, address.RegionId);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressRegionName, address.RegionName);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressCity, address.City);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressLine1, address.Line1);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressLine2, address.Line2);
        document.AddFilterableStringAndContentString(PickupLocationIndexFields.AddressPostalCode, address.PostalCode);
    }
}
