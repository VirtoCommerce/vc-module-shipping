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
        protected const string DefaultSortColumn = nameof(StoreShippingMethodEntity.Code);

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
                // Plain LINQ-to-objects: composing operators on an in-memory IQueryable
                // (EnumerableQuery) rebuilds and compiles an expression tree on every
                // enumeration; this method runs on every cart/checkout read, and the per-call
                // compilation convoys on runtime-wide locks under concurrent requests.
                var transientMethods = AbstractTypeFactory<ShippingMethod>.AllTypeInfos
                    .Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name));

                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    transientMethods = transientMethods.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
                }

                if (!criteria.Codes.IsNullOrEmpty())
                {
                    transientMethods = transientMethods.Where(x => criteria.Codes.Contains(x.Code));
                }

                if (!criteria.TaxType.IsNullOrEmpty())
                {
                    transientMethods = transientMethods.Where(x => criteria.TaxType.Contains(x.TaxType));
                }

                if (criteria.IsActive.HasValue)
                {
                    transientMethods = transientMethods.Where(x => x.IsActive == criteria.IsActive.Value);
                }

                var persistentMethodTypes = result.Results.Select(x => x.GetType()).ToHashSet();
                var filteredTransientMethods = transientMethods
                    .Where(x => !persistentMethodTypes.Contains(x.GetType()))
                    .ToList();

                result.TotalCount += filteredTransientMethods.Count;

                var pagedTransientMethods = filteredTransientMethods
                    .Skip(criteria.Skip)
                    .Take(criteria.Take)
                    .ToList();

                foreach (var transientMethod in pagedTransientMethods)
                {
                    await _settingsManager.DeepLoadSettingsAsync(transientMethod);
                }

                var allMethods = result.Results.Concat(pagedTransientMethods);

                // Arbitrary sort columns (admin, cold) are worth OrderBySortInfos' compile; the default
                // order is not. Decided from what BuildSortExpression returned, so overriding that seam
                // still changes the sort.
                result.Results = IsSingleAscendingDefaultSort(sortInfos)
                    ? allMethods.OrderBy(x => x.Code).ToList()
                    : allMethods.AsQueryable().OrderBySortInfos(sortInfos).ToList();
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
                    new SortInfo{ SortColumn = DefaultSortColumn }
                };
            }

            return sortInfos;
        }

        protected static bool IsSingleAscendingDefaultSort(IList<SortInfo> sortInfos)
        {
            return sortInfos?.Count == 1
                && sortInfos[0].SortDirection == SortDirection.Ascending
                && DefaultSortColumn.EqualsIgnoreCase(sortInfos[0].SortColumn);
        }
    }
}
