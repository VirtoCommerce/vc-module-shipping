using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Exceptions;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Services;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Extensions;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;
using VirtoCommerce.ShippingModule.Core.Search.Indexed;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationIndexedSearchService(
    ISearchRequestBuilderRegistrar searchRequestBuilderRegistrar,
    ISearchProvider searchProvider,
    IPickupLocationService pickupLocationService,
    IConfiguration configuration
    )
: IPickupLocationIndexedSearchService
{
    public virtual async Task<PickupLocationIndexedSearchResult> SearchPickupLocationsAsync(PickupLocationIndexedSearchCriteria searchCriteria)
    {
        if (!configuration.IsPickupLocationFullTextSearchEnabled())
        {
            throw new SearchException("Indexed order search is disabled. To enable it add 'Search:PickupLocationFullTextSearchEnabled' configuration key to app settings and set it to true.");
        }

        var requestBuilder = searchRequestBuilderRegistrar.GetRequestBuilderByDocumentType(ModuleConstants.PickupLocationIndexDocumentType);
        var request = await requestBuilder.BuildRequestAsync(searchCriteria);

        var response = await searchProvider.SearchAsync(ModuleConstants.PickupLocationIndexDocumentType, request);

        var result = await ConvertResponseAsync(response, searchCriteria, request);
        return result;
    }

    protected virtual async Task<PickupLocationIndexedSearchResult> ConvertResponseAsync(SearchResponse response, PickupLocationIndexedSearchCriteria searchCriteria, SearchRequest searchRequest)
    {
        var result = AbstractTypeFactory<PickupLocationIndexedSearchResult>.TryCreateInstance();

        if (response != null)
        {
            result.TotalCount = (int)response.TotalCount;
            result.Results = await ConvertDocumentsAsync(response.Documents, searchCriteria);
            result.Aggregations = await ConvertAggregationsAsync(response.Aggregations, searchRequest);
        }

        return result;
    }

    protected virtual async Task<IList<PickupLocation>> ConvertDocumentsAsync(IList<SearchDocument> documents, PickupLocationIndexedSearchCriteria searchCriteria)
    {
        var result = new List<PickupLocation>();

        if (documents?.Any() != true)
        {
            return result;
        }

        var itemIds = documents.Select(x => x.Id).ToArray();
        var items = await pickupLocationService.GetAsync(itemIds, searchCriteria.ResponseGroup);
        var itemsMap = items.ToDictionary(x => x.Id, x => x);

        // Preserve documents order
        var orders = documents
            .Select(doc =>
            {
                var order = itemsMap.TryGetValue(doc.Id, out var value) ? value : null;

                if (order != null)
                {
                    order.RelevanceScore = doc.GetRelevanceScore();
                }

                return order;
            })
            .Where(x => x != null)
            .ToArray();

        result.AddRange(orders);

        return result;
    }

    private async Task<IList<Aggregation>> ConvertAggregationsAsync(IList<AggregationResponse> aggregationResponses, SearchRequest searchRequest)
    {
        var result = new List<Aggregation>();

        foreach (var aggregationRequest in searchRequest.Aggregations)
        {
            var aggregationResponse = aggregationResponses.FirstOrDefault(x => x.Id == aggregationRequest.Id);
            if (aggregationResponse != null)
            {
                var orderAggregation = default(Aggregation);

                if (aggregationRequest is RangeAggregationRequest rangeAggregationRequest)
                {
                    orderAggregation = new Aggregation()
                    {
                        AggregationType = "range",
                        Field = aggregationRequest.FieldName,
                        Items = GetAttributeAggregationItems(rangeAggregationRequest, aggregationResponse.Values),
                    };
                }
                else if (aggregationRequest is TermAggregationRequest)
                {
                    orderAggregation = new Aggregation()
                    {
                        AggregationType = "attr",
                        Field = aggregationRequest.FieldName,
                        Items = await GetAttributeAggregationItemsAsync(aggregationResponse.Values),
                    };
                }

                result.Add(orderAggregation);
            }
        }

        searchRequest.SetAppliedAggregations(result);

        return result;
    }

    private static List<AggregationItem> GetAttributeAggregationItems(RangeAggregationRequest rangeAggregationRequest, IList<AggregationResponseValue> resultValues)
    {
        var result = new List<AggregationItem>();

        foreach (var requestValue in rangeAggregationRequest.Values)
        {
            var resultValue = resultValues.FirstOrDefault(x => x.Id == requestValue.Id);
            if (resultValue != null)
            {
                var aggregationItem = new AggregationItem
                {
                    Value = resultValue.Id,
                    Count = (int)resultValue.Count,
                    RequestedLowerBound = requestValue.Lower,
                    RequestedUpperBound = requestValue.Upper,
                    IncludeLower = requestValue.IncludeLower,
                    IncludeUpper = requestValue.IncludeUpper,
                };

                result.Add(aggregationItem);
            }
        }

        return result;
    }

    private Task<IList<AggregationItem>> GetAttributeAggregationItemsAsync(IList<AggregationResponseValue> aggregationResponseValues)
    {
        var result = aggregationResponseValues
            .Select(x =>
            {
                var item = new AggregationItem
                {
                    Value = x.Id,
                    Count = (int)x.Count,
                };

                return item;
            })
            .ToList();

        return Task.FromResult<IList<AggregationItem>>(result);
    }
}
