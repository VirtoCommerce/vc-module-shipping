using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Services;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model.Search.Indexed;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationSearchRequestBuilder(ISearchPhraseParser searchPhraseParser)
    : IndexedSearchRequestBuilder<PickupLocationIndexedSearchCriteria>(searchPhraseParser)
{
    private readonly ISearchPhraseParser _searchPhraseParser = searchPhraseParser;

    public override string DocumentType => ModuleConstants.PickupLocationIndexDocumentType;

    protected override IList<IFilter> GetFilters(PickupLocationIndexedSearchCriteria criteria)
    {
        var result = base.GetFilters(criteria);

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            result.Add(FilterHelper.CreateTermFilter("StoreId", criteria.StoreId));
        }

        if (criteria.IsActive.HasValue)
        {
            result.Add(FilterHelper.CreateBoolFilter("IsActive", criteria.IsActive.Value));
        }

        if (!criteria.Filter.IsNullOrEmpty())
        {
            var parseResult = _searchPhraseParser.Parse(criteria.Filter);
            result.AddRange(parseResult.Filters);
        }

        return result;
    }
}
