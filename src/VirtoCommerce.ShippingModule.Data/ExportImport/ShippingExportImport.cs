using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.ExportImport;

public class ShippingExportImport(
    IShippingMethodsService shippingMethodsService,
    IPickupLocationService pickupLocationService,
    IShippingMethodsSearchService shippingMethodsSearchService,
    IPickupLocationSearchService pickupLocationSearchService,
    JsonSerializer jsonSerializer)
{
    private const int _batchSize = 50;

    public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var progressInfo = new ExportImportProgressInfo { Description = "Shipping methods are loading" };
        progressCallback(progressInfo);

        await using var sw = new StreamWriter(outStream);
        await using var writer = new JsonTextWriter(sw);

        await writer.WriteStartObjectAsync();

        progressInfo.Description = "Shipping methods are started to export";
        progressCallback(progressInfo);

        await writer.WritePropertyNameAsync("ShippingMethods");
        await writer.SerializeArrayWithPagingAsync(jsonSerializer, _batchSize, async (skip, take) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var searchCriteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
            searchCriteria.Take = take;
            searchCriteria.Skip = skip;
            searchCriteria.WithoutTransient = true;

            var searchResult = await shippingMethodsSearchService.SearchNoCloneAsync(searchCriteria);
            return (GenericSearchResult<ShippingMethod>)searchResult;
        }, (processedCount, totalCount) =>
        {
            progressInfo.Description = $"{processedCount} of {totalCount} shipping methods have been exported";
            progressCallback(progressInfo);
        }, cancellationToken);

        await writer.WritePropertyNameAsync("PickupLocations");
        await writer.SerializeArrayWithPagingAsync(jsonSerializer, _batchSize, async (skip, take) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var searchCriteria = AbstractTypeFactory<PickupLocationSearchCriteria>.TryCreateInstance();
            searchCriteria.Take = take;
            searchCriteria.Skip = skip;

            var searchResult = await pickupLocationSearchService.SearchNoCloneAsync(searchCriteria);
            return (GenericSearchResult<PickupLocation>)searchResult;
        }, (processedCount, totalCount) =>
        {
            progressInfo.Description = $"{processedCount} of {totalCount} pickup locations have been exported";
            progressCallback(progressInfo);
        }, cancellationToken);

        await writer.WriteEndObjectAsync();
        await writer.FlushAsync();
    }

    public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var progressInfo = new ExportImportProgressInfo();

        using var streamReader = new StreamReader(inputStream);
        await using var reader = new JsonTextReader(streamReader);
        while (await reader.ReadAsync())
        {
            if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "ShippingMethods")
            {
                try
                {
                    await reader.DeserializeArrayWithPagingAsync<ShippingMethod>(jsonSerializer, _batchSize,
                        shippingMethodsService.SaveChangesAsync,
                        processedCount =>
                        {
                            progressInfo.Description = $"{processedCount} shipping methods have been imported";
                            progressCallback(progressInfo);
                            cancellationToken.ThrowIfCancellationRequested();
                        }, cancellationToken);
                }
                catch (Exception ex)
                {
                    progressInfo.Errors.Add($"Warning. Could not deserialize shipping method. More details: {ex}");
                }
            }

            if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "PickupLocations")
            {
                try
                {
                    await reader.DeserializeArrayWithPagingAsync<PickupLocation>(jsonSerializer, _batchSize,
                        pickupLocationService.SaveChangesAsync,
                        processedCount =>
                        {
                            progressInfo.Description = $"{processedCount} pickup locations have been imported";
                            progressCallback(progressInfo);
                            cancellationToken.ThrowIfCancellationRequested();
                        }, cancellationToken);
                }
                catch (Exception ex)
                {
                    progressInfo.Errors.Add($"Warning. Could not deserialize pickup location. More details: {ex}");
                }
            }
        }
    }
}
