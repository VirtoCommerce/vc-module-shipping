using Microsoft.Extensions.Configuration;
using VirtoCommerce.SearchModule.Core.Exceptions;
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
    IConfiguration configuration)
    : IndexedSearchService<PickupLocationIndexedSearchCriteria, PickupLocationIndexedSearchResult, PickupLocation>
        (pickupLocationService, searchProvider, searchRequestBuilderRegistrar),
        IPickupLocationIndexedSearchService
{
    public override string DocumentType => ModuleConstants.PickupLocationIndexDocumentType;

    protected override void EnsureIndexedSearchEnabled()
    {
        if (!configuration.IsPickupLocationFullTextSearchEnabled())
        {
            throw new SearchException("Indexed search is disabled. To enable it, add the 'Search:PickupLocationFullTextSearchEnabled' configuration key to the app settings and set its value to true.");
        }
    }
}
