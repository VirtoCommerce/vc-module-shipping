using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationDocumentBuilder(IPickupLocationService pickupLocationService)
    : IIndexSchemaBuilder, IIndexDocumentBuilder
{
    public Task BuildSchemaAsync(IndexDocument schema)
    {
        schema.AddFilterableString("StoreId");
        schema.AddFilterableString("OuterId");

        schema.AddFilterableBoolean("IsActive");

        schema.AddFilterableValue("GeoLocation", null, IndexDocumentFieldValueType.GeoPoint);

        schema.AddFilterableStringAndContentString("Name");
        schema.AddFilterableStringAndContentString("Description");
        schema.AddFilterableStringAndContentString("WorkingHours");
        schema.AddFilterableStringAndContentString("ContactEmail");
        schema.AddFilterableStringAndContentString("ContactPhone");

        schema.AddFilterableStringAndContentString("Address_CountryCode");
        schema.AddFilterableStringAndContentString("Address_CountryName");
        schema.AddFilterableStringAndContentString("Address_RegionId");
        schema.AddFilterableStringAndContentString("Address_RegionName");
        schema.AddFilterableStringAndContentString("Address_City");
        schema.AddFilterableStringAndContentString("Address_Line1");
        schema.AddFilterableStringAndContentString("Address_Line2");
        schema.AddFilterableStringAndContentString("Address_PostalCode");

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

        document.AddFilterableString("StoreId", pickupLocation.StoreId);
        document.AddFilterableString("OuterId", pickupLocation.OuterId);

        document.AddFilterableBoolean("IsActive", pickupLocation.IsActive);

        if (!pickupLocation.GeoLocation.IsNullOrEmpty())
        {
            document.AddFilterableValue("GeoLocation", GeoPoint.TryParse(pickupLocation.GeoLocation), IndexDocumentFieldValueType.GeoPoint);
        }

        document.AddFilterableStringAndContentString("Name", pickupLocation.Name);
        document.AddFilterableStringAndContentString("Description", pickupLocation.Description);
        document.AddFilterableStringAndContentString("WorkingHours", pickupLocation.WorkingHours);
        document.AddFilterableStringAndContentString("ContactEmail", pickupLocation.ContactEmail);
        document.AddFilterableStringAndContentString("ContactPhone", pickupLocation.ContactPhone);

        if (pickupLocation.Address != null)
        {
            IndexAddress(pickupLocation.Address, document);
        }

        return Task.FromResult(document);
    }

    protected virtual void IndexAddress(PickupLocationAddress address, IndexDocument document)
    {
        document.AddFilterableStringAndContentString("Address_CountryCode", address.CountryCode);
        document.AddFilterableStringAndContentString("Address_CountryName", address.CountryName);
        document.AddFilterableStringAndContentString("Address_RegionId", address.RegionId);
        document.AddFilterableStringAndContentString("Address_RegionName", address.RegionName);
        document.AddFilterableStringAndContentString("Address_City", address.City);
        document.AddFilterableStringAndContentString("Address_Line1", address.Line1);
        document.AddFilterableStringAndContentString("Address_Line2", address.Line2);
        document.AddFilterableStringAndContentString("Address_PostalCode", address.PostalCode);
    }
}
