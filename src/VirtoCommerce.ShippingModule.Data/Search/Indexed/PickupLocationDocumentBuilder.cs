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

        document.AddFilterableValue("GeoLocation", GeoPoint.TryParse(pickupLocation.GeoLocation), IndexDocumentFieldValueType.GeoPoint);

        document.AddFilterableStringAndContentString("Name", pickupLocation.Name);
        document.AddFilterableStringAndContentString("Description", pickupLocation.Description);
        document.AddFilterableStringAndContentString("WorkingHours", pickupLocation.WorkingHours);
        document.AddFilterableStringAndContentString("ContactEmail", pickupLocation.ContactEmail);
        document.AddFilterableStringAndContentString("ContactPhone", pickupLocation.ContactPhone);

        if (pickupLocation.Address != null)
        {
            document.AddFilterableStringAndContentString("Address_CountryCode", pickupLocation.Address.CountryCode);
            document.AddFilterableStringAndContentString("Address_CountryName", pickupLocation.Address.CountryName);
            document.AddFilterableStringAndContentString("Address_RegionId", pickupLocation.Address.RegionId);
            document.AddFilterableStringAndContentString("Address_RegionName", pickupLocation.Address.RegionName);
            document.AddFilterableStringAndContentString("Address_City", pickupLocation.Address.City);
            document.AddFilterableStringAndContentString("Address_Line1", pickupLocation.Address.Line1);
            document.AddFilterableStringAndContentString("Address_Line2", pickupLocation.Address.Line2);
            document.AddFilterableStringAndContentString("Address_PostalCode", pickupLocation.Address.PostalCode);
        }

        return Task.FromResult(document);
    }

    protected virtual void IndexAddress(PickupLocationAddress address, IndexDocument document)
    {
        if (address != null)
        {
            document.AddContentString($"{address.AddressType} {address}");
        }
    }
}
