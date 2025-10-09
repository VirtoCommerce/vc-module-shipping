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
        schema.AddFilterableString("Name");
        schema.AddFilterableString("Description");
        schema.AddFilterableString("WorkingHours");
        schema.AddFilterableString("ContactEmail");
        schema.AddFilterableString("ContactPhone");
        schema.AddFilterableString("StoreId");
        schema.AddFilterableString("OuterId");

        schema.AddFilterableBoolean("IsActive");

        schema.AddFilterableString("Address_CountryCode");
        schema.AddFilterableString("Address_CountryName");
        schema.AddFilterableString("Address_RegionId");
        schema.AddFilterableString("Address_RegionName");
        schema.AddFilterableString("Address_City");
        schema.AddFilterableString("Address_Line1");
        schema.AddFilterableString("Address_Line2");
        schema.AddFilterableString("Address_PostalCode");

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

        document.AddFilterableString("Name", pickupLocation.Name);
        document.AddFilterableString("Description", pickupLocation.Description);
        document.AddFilterableString("WorkingHours", pickupLocation.WorkingHours);
        document.AddFilterableString("ContactEmail", pickupLocation.ContactEmail);
        document.AddFilterableString("ContactPhone", pickupLocation.ContactPhone);
        document.AddFilterableString("StoreId", pickupLocation.StoreId);
        document.AddFilterableString("OuterId", pickupLocation.OuterId);

        document.AddFilterableBoolean("IsActive", pickupLocation.IsActive);

        if (pickupLocation.Address != null)
        {
            document.AddFilterableString("Address_CountryCode", pickupLocation.Address.CountryCode);
            document.AddFilterableString("Address_CountryName", pickupLocation.Address.CountryName);
            document.AddFilterableString("Address_RegionId", pickupLocation.Address.RegionId);
            document.AddFilterableString("Address_RegionName", pickupLocation.Address.RegionName);
            document.AddFilterableString("Address_City", pickupLocation.Address.City);
            document.AddFilterableString("Address_Line1", pickupLocation.Address.Line1);
            document.AddFilterableString("Address_Line2", pickupLocation.Address.Line2);
            document.AddFilterableString("Address_PostalCode", pickupLocation.Address.PostalCode);
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
