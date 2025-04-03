using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsSearchService : SearchService<ShippingMethodsSearchCriteria, ShippingMethodsSearchResult, ShippingMethod, StoreShippingMethodEntity>, IShippingMethodsSearchService
    {
        private readonly ISettingsManager _settingsManager;

        public ShippingMethodsSearchService(
            Func<IShippingRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IShippingMethodsService crudService,
            IOptions<CrudOptions> crudOptions,
            ISettingsManager settingsManager)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
            _settingsManager = settingsManager;
        }

        protected override async Task<ShippingMethodsSearchResult> ProcessSearchResultAsync(ShippingMethodsSearchResult result, ShippingMethodsSearchCriteria criteria)
        {
            var sortInfos = BuildSortExpression(criteria);

            if (criteria.Take > 0 && !criteria.WithoutTransient)
            {
                var transientMethodsQuery = AbstractTypeFactory<ShippingMethod>.AllTypeInfos
                    .Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    transientMethodsQuery = transientMethodsQuery.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
                }

                if (!criteria.Codes.IsNullOrEmpty())
                {
                    transientMethodsQuery = transientMethodsQuery.Where(x => criteria.Codes.Contains(x.Code));
                }

                if (!criteria.TaxType.IsNullOrEmpty())
                {
                    transientMethodsQuery = transientMethodsQuery.Where(x => criteria.TaxType.Contains(x.TaxType));
                }

                if (criteria.IsActive.HasValue)
                {
                    transientMethodsQuery = transientMethodsQuery.Where(x => x.IsActive == criteria.IsActive.Value);
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

            // Return only registered shipping methods
            var registeredShippingMethods = AbstractTypeFactory<ShippingMethod>.AllTypeInfos.Select(x => x.TypeName).ToArray();
            query = query.Where(x => registeredShippingMethods.Contains(x.Code));


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
