using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.ExportImport
{
    public class ShippingExportImport
    {
        private readonly IShippingMethodsService _shippingMethodsService;
        private readonly IPickupLocationsService _pickupLocationsService;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IPickupLocationsSearchService _pickupLocationSearchService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly int _batchSize = 50;

        public ShippingExportImport(
            IShippingMethodsService shippingMethodsService,
            IPickupLocationsService pickupLocationsService,
            IShippingMethodsSearchService shippingMethodsSearchService,
            IPickupLocationsSearchService pickupLocationSearchService,
            JsonSerializer jsonSerializer)
        {
            _shippingMethodsService = shippingMethodsService;
            _pickupLocationsService = pickupLocationsService;
            _jsonSerializer = jsonSerializer;
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _pickupLocationSearchService = pickupLocationSearchService;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "Shipping methods are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Shipping methods are started to export";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("ShippingMethods");
                await writer.SerializeArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var searchCriteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;
                    searchCriteria.WithoutTransient = true;

                    var searchResult = await _shippingMethodsSearchService.SearchNoCloneAsync(searchCriteria);
                    return (GenericSearchResult<ShippingMethod>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} shipping methods have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WritePropertyNameAsync("PickupLocations");
                await writer.SerializeArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var searchCriteria = AbstractTypeFactory<PickupLocationsSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;

                    var searchResult = await _pickupLocationSearchService.SearchNoCloneAsync(searchCriteria);
                    return (GenericSearchResult<PickupLocation>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} pickup locations have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using var streamReader = new StreamReader(inputStream);
            await using var reader = new JsonTextReader(streamReader);
            while (await reader.ReadAsync())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "ShippingMethods")
                {
                    try
                    {
                        await reader.DeserializeArrayWithPagingAsync<ShippingMethod>(_jsonSerializer, _batchSize,
                            items => _shippingMethodsService.SaveChangesAsync(items), processedCount =>
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

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "PickupLocations")
                {
                    try
                    {
                        await reader.DeserializeArrayWithPagingAsync<PickupLocation>(_jsonSerializer, _batchSize,
                            items => _pickupLocationsService.SaveChangesAsync(items), processedCount =>
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
}
