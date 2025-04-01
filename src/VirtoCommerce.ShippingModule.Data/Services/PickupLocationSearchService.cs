using System;
using System.Collections.Generic;
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

public class PickupLocationSearchService(Func<IShippingRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IPickupLocationService crudService,
    IOptions<CrudOptions> crudOptions)
    : SearchService<PickupLocationSearchCriteria, PickupLocationSearchResult, PickupLocation, PickupLocationEntity>
        (repositoryFactory, platformMemoryCache, crudService, crudOptions),
        IPickupLocationSearchService
{
    protected override IQueryable<PickupLocationEntity> BuildQuery(IRepository repository, PickupLocationSearchCriteria criteria)
    {
        var query = ((IShippingRepository)repository).PickupLocations;

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


    protected override IList<SortInfo> BuildSortExpression(PickupLocationSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = [new SortInfo { SortColumn = nameof(PickupLocationEntity.CreatedDate), SortDirection = SortDirection.Descending }];
        }

        return sortInfos;
    }
}
