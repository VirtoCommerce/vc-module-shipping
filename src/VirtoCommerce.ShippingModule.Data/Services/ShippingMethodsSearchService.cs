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
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsSearchService : SearchService<ShippingMethodsSearchCriteria, ShippingMethodsSearchResult, ShippingMethod, StoreShippingMethodEntity>, IShippingMethodsSearchService
    {
        private readonly ISettingsManager _settingsManager;

        public ShippingMethodsSearchService(Func<IShippingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IShippingMethodsService customerReviewService, ISettingsManager settingsManager)
            : base(repositoryFactory, platformMemoryCache, (ICrudService<ShippingMethod>)customerReviewService)
        {
            _settingsManager = settingsManager;
        }

        public async Task<ShippingMethodsSearchResult> SearchShippingMethodsAsync(ShippingMethodsSearchCriteria criteria)
        {
            var result = await SearchAsync(criteria);
            var sortInfos = BuildSortExpression(criteria);

            if (criteria.Take > 0 && !criteria.WithoutTransient)
            {
                var transientMethodsQuery = AbstractTypeFactory<ShippingMethod>.AllTypeInfos.Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                                                                              .OfType<ShippingMethod>().AsQueryable();
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    transientMethodsQuery = transientMethodsQuery.Where(x => x.Code.Contains(criteria.Keyword));
                }
                var allPersistentTypes = result.Results.Select(x => x.GetType()).Distinct();
                transientMethodsQuery = transientMethodsQuery.Where(x => !allPersistentTypes.Contains(x.GetType()));

                result.TotalCount += transientMethodsQuery.Count();
                var transientProviders = transientMethodsQuery.Skip(criteria.Skip).Take(criteria.Take).ToList();

                foreach (var transientProvider in transientProviders)
                {
                    await _settingsManager.DeepLoadSettingsAsync(transientProvider);
                }

                result.Results = result.Results.Concat(transientProviders).AsQueryable().OrderBySortInfos(sortInfos).ToList();
            }
            return result;
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
