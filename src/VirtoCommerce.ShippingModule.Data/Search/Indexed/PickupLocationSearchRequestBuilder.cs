using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model.Search;

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
        criteria = criteria.CloneTyped();
        var filter = GetFilters(criteria).And();
        var aggregations = GetAggregations(criteria);
        aggregations = ApplyMultiSelectFacetSearch(aggregations, filter);

        var request = new SearchRequest
        {
            SearchKeywords = criteria.Keyword,
            SearchFields = new[] { IndexDocumentExtensions.ContentFieldName },
            Filter = filter,
            Aggregations = aggregations,
            Sorting = GetSorting(criteria),
            Skip = criteria.Skip,
            Take = criteria.Take,
        };

        return Task.FromResult(request);
    }

    private List<AggregationRequest> GetAggregations(SearchCriteriaBase criteria)
    {
        var result = new List<AggregationRequest>();

        if (criteria is not PickupLocationIndexedSearchCriteria indexedSearchCriteria || string.IsNullOrEmpty(indexedSearchCriteria.Facet))
        {
            return result;
        }

        var parseResult = _searchPhraseParser.Parse(indexedSearchCriteria.Facet);
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
                        //Id = filter.Stringify(true),//TODO
                        FieldName = rangeFilter.FieldName,
                        Values = rangeFilter.Values.Select(x => new RangeAggregationRequestValue
                        {
                            //Id = x.Stringify(),//TODO
                            Lower = x.Lower,
                            Upper = x.Upper,
                            IncludeLower = x.IncludeLower,
                            IncludeUpper = x.IncludeUpper
                        }).ToList()
                    },
                    TermFilter termFilter => new TermAggregationRequest
                    {
                        FieldName = termFilter.FieldName,
                        //Id = filter.Stringify(),//TODO
                        Size = 0
                    },
                    _ => null,
                };
            })
            .Where(x => x != null)
            .ToList();

        return result;
    }

    protected virtual IList<IFilter> GetFilters(SearchCriteriaBase criteria)
    {
        var result = new List<IFilter>();

        if (criteria.ObjectIds?.Any() == true)
        {
            result.Add(new IdsFilter { Values = criteria.ObjectIds });
        }

        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            var parseResult = _searchPhraseParser.Parse(criteria.Keyword);
            criteria.Keyword = parseResult.Keyword;
            result.AddRange(parseResult.Filters);
        }

        if (criteria is PickupLocationIndexedSearchCriteria pickupLocationIndexedSearchCriteria)
        {
            result.AddRange(GetPermanentFilters(pickupLocationIndexedSearchCriteria));
        }

        return result;
    }

    protected virtual IList<IFilter> GetPermanentFilters(PickupLocationIndexedSearchCriteria searchCriteria)
    {
        var result = new List<IFilter>();

        if (!searchCriteria.CountryCode.IsNullOrEmpty())
        {
            //TODO: move FilterHelper to Core
            //result.Add(FilterHelper.CreateTermFilter("CountryCode", criteria.CountryCode));
        }

        if (!searchCriteria.RegionId.IsNullOrEmpty())
        {
            //TODO: move FilterHelper to Core
            //result.Add(FilterHelper.CreateTermFilter("RegionId", criteria.RegionId));
        }

        if (!searchCriteria.City.IsNullOrEmpty())
        {
            //TODO: move FilterHelper to Core
            //result.Add(FilterHelper.CreateTermFilter("City", criteria.City));
        }

        if (!searchCriteria.PostalCode.IsNullOrEmpty())
        {
            //TODO: move FilterHelper to Core
            //result.Add(FilterHelper.CreateTermFilter("PostalCode", criteria.PostalCode));
        }

        return result;
    }


    protected virtual IList<SortingField> GetSorting(SearchCriteriaBase criteria)
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
