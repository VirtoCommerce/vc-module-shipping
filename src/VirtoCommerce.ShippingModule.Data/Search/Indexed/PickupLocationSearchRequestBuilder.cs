using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Services;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationSearchRequestBuilder : ISearchRequestBuilder
{
    public string DocumentType { get; } = ModuleConstants.PickupLocationIndexDocumentType;

    private readonly ISearchPhraseParser _searchPhraseParser;

    public PickupLocationSearchRequestBuilder(ISearchPhraseParser searchPhraseParser)
    {
        _searchPhraseParser = searchPhraseParser;
    }

    public Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria)
    {
        SearchRequest result = null;

        if (criteria is not PickupLocationIndexedSearchCriteria pickupLocationIndexedSearchCriteria)
        {
            return Task.FromResult(result);
        }

        pickupLocationIndexedSearchCriteria = pickupLocationIndexedSearchCriteria.CloneTyped();
        var filter = GetFilters(pickupLocationIndexedSearchCriteria).And();
        var aggregations = GetAggregations(pickupLocationIndexedSearchCriteria);
        aggregations = ApplyMultiSelectFacetSearch(aggregations, filter);

        result = new SearchRequest
        {
            SearchKeywords = pickupLocationIndexedSearchCriteria.Keyword,
            SearchFields = new[] { IndexDocumentExtensions.ContentFieldName },
            Filter = filter,
            Aggregations = aggregations,
            Sorting = GetSorting(pickupLocationIndexedSearchCriteria),
            Skip = pickupLocationIndexedSearchCriteria.Skip,
            Take = pickupLocationIndexedSearchCriteria.Take,
        };

        return Task.FromResult(result);
    }

    private List<AggregationRequest> GetAggregations(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = new List<AggregationRequest>();

        if (criteria.Facet.IsNullOrEmpty())
        {
            return result;
        }

        var parseResult = _searchPhraseParser.Parse(criteria.Facet);
        if (!string.IsNullOrEmpty(parseResult.Keyword))
        {
            var termFacetExpressions = parseResult.Keyword.Split(" ");
            parseResult.Filters.AddRange(termFacetExpressions.Select(x => new TermFilter
            {
                FieldName = x,
                Values = new List<string>()
            }));
        }

        result = parseResult.Filters
            .Select<IFilter, AggregationRequest>(filter =>
            {
                return filter switch
                {
                    RangeFilter rangeFilter => new RangeAggregationRequest
                    {
                        Id = filter.Stringify(true),
                        FieldName = rangeFilter.FieldName,
                        Values = rangeFilter.Values.Select(x => new RangeAggregationRequestValue
                        {
                            Id = x.Stringify(),
                            Lower = x.Lower,
                            Upper = x.Upper,
                            IncludeLower = x.IncludeLower,
                            IncludeUpper = x.IncludeUpper
                        }).ToList()
                    },
                    TermFilter termFilter => new TermAggregationRequest
                    {
                        FieldName = termFilter.FieldName,
                        Id = filter.Stringify(),
                        Size = 0
                    },
                    _ => null,
                };
            })
            .Where(x => x != null)
            .ToList();

        return result;
    }

    protected virtual IList<IFilter> GetFilters(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = new List<IFilter>();

        result.AddRange(GetOptionalFilters(criteria));
        result.AddRange(GetPermanentFilters(criteria));

        return result;
    }

    protected virtual IList<IFilter> GetOptionalFilters(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = new List<IFilter>();

        if (!criteria.Filter.IsNullOrEmpty())
        {
            var parseResult = _searchPhraseParser.Parse(criteria.Filter);
            result.AddRange(parseResult.Filters);
        }

        return result;
    }

    protected virtual IList<IFilter> GetPermanentFilters(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = new List<IFilter>();

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            result.Add(FilterHelper.CreateTermFilter("StoreId", criteria.StoreId));
        }

        return result;
    }


    protected virtual IList<SortingField> GetSorting(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = new List<SortingField>();

        foreach (var sortInfo in criteria.SortInfos)
        {
            var fieldName = sortInfo.SortColumn.ToLowerInvariant();
            var isDescending = sortInfo.SortDirection == SortDirection.Descending;
            result.Add(new SortingField(fieldName, isDescending));
        }

        return result;
    }

    public static List<AggregationRequest> ApplyMultiSelectFacetSearch(List<AggregationRequest> aggregations, IFilter filter)
    {
        foreach (var aggregation in aggregations)
        {
            var aggregationFilterFieldName = aggregation.FieldName ?? (aggregation.Filter as INamedFilter)?.FieldName;

            var clonedFilter = filter.Clone() as AndFilter;
            if (clonedFilter == null)
            {
                clonedFilter = new AndFilter { ChildFilters = new List<IFilter>() { clonedFilter } };
            }

            // For multi-select facet mechanism, we should select
            // search request filters which do not have the same
            // names such as aggregation filter
            clonedFilter.ChildFilters = clonedFilter.ChildFilters
                .Where(x =>
                {
                    var result = true;

                    if (x is INamedFilter namedFilter)
                    {
                        result = !(aggregationFilterFieldName?.StartsWith(namedFilter.FieldName, true, CultureInfo.InvariantCulture) ?? false);
                    }

                    return result;
                })
                .ToList();

            aggregation.Filter = aggregation.Filter == null ? clonedFilter : aggregation.Filter.And(clonedFilter);
        }

        return aggregations;
    }
}
