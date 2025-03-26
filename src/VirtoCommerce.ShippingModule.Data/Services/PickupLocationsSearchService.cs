using System;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services;

public class PickupLocationsSearchService(Func<IPickupLocationsRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IPickupLocationsService crudService,
    IOptions<CrudOptions> crudOptions)
    : SearchService<PickupLocationsSearchCriteria, PickupLocationsSearchResult, PickupLocation, PickupLocationEntity>
        (repositoryFactory, platformMemoryCache, crudService, crudOptions),
        IPickupLocationsSearchService
{
    protected override IQueryable<PickupLocationEntity> BuildQuery(IRepository repository, PickupLocationsSearchCriteria criteria)
    {
        var query = ((IPickupLocationsRepository)repository).PickupLocations;

        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
        }

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        return query;
    }
}
