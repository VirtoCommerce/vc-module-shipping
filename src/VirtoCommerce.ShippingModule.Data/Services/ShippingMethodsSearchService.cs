using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsSearchService : SearchService<ShippingMethodsSearchCriteria, ShippingMethodsSearchResult, ShippingMethod, StoreShippingMethodEntity>, IShippingMethodsSearchService
    {
        public ShippingMethodsSearchService(Func<IShippingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IShippingMethodsService customerReviewService)
            : base(repositoryFactory, platformMemoryCache, (ICrudService<ShippingMethod>)customerReviewService)
        {
        }

        protected override IQueryable<StoreShippingMethodEntity> BuildQuery(IRepository repository, ShippingMethodsSearchCriteria criteria)
        {
            var query = ((IShippingRepository)repository).ShippingMethods;

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
            }

            if (!criteria.StoreId.IsNullOrEmpty())
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }

            if (!criteria.Codes.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Codes.Contains(x.Code));
            }

            if (!criteria.TaxType.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.TaxType.Contains(x.TaxType));
            }

            if (criteria.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == criteria.IsActive.Value);
            }
            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(ShippingMethodsSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo{ SortColumn = nameof(StoreShippingMethodEntity.Code) }
                };
            }

            return sortInfos;
        }
    }
}
