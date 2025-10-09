using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Search.Indexed;

public class PickupLocationChangesProvider(Func<IShippingRepository> shippingRepositoryFactory, IChangeLogSearchService changeLogSearchService)
    : IIndexDocumentChangesProvider
{
    public const string ChangeLogObjectType = nameof(PickupLocation);

    public async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
    {
        if (startDate == null && endDate == null)
        {
            return GetChangesFromRepository(skip, take);
        }

        return await GetChangesFromOperaionLog(startDate, endDate, skip, take);
    }

    public async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
    {
        if (startDate == null && endDate == null)
        {
            using (var repository = shippingRepositoryFactory())
            {
                return repository.PickupLocations.Count();
            }
        }

        var criteria = GetChangeLogSearchCriteria(startDate, endDate, 0, 0);

        return (await changeLogSearchService.SearchAsync(criteria)).TotalCount;
    }

    private IList<IndexDocumentChange> GetChangesFromRepository(long skip, long take)
    {
        using (var repository = shippingRepositoryFactory())
        {
            var pickupLocationIds = repository.PickupLocations
                .OrderBy(x => x.CreatedDate)
                .Select(x => x.Id)
                .Skip((int)skip)
                .Take((int)take)
                .ToArray();

            return pickupLocationIds
                .Select(id => new IndexDocumentChange
                {
                    DocumentId = id,
                    ChangeType = IndexDocumentChangeType.Modified,
                    ChangeDate = DateTime.UtcNow
                })
                .ToArray();
        }
    }

    private async Task<IList<IndexDocumentChange>> GetChangesFromOperaionLog(DateTime? startDate, DateTime? endDate, long skip, long take)
    {
        var criteria = GetChangeLogSearchCriteria(startDate, endDate, skip, take);
        var operations = (await changeLogSearchService.SearchAsync(criteria)).Results;

        return operations.Select(o =>
            new IndexDocumentChange
            {
                DocumentId = o.ObjectId,
                ChangeType = o.OperationType == EntryState.Deleted ? IndexDocumentChangeType.Deleted : IndexDocumentChangeType.Modified,
                ChangeDate = o.ModifiedDate ?? o.CreatedDate,
            }
        ).ToArray();
    }

    protected virtual ChangeLogSearchCriteria GetChangeLogSearchCriteria(DateTime? startDate, DateTime? endDate, long skip, long take)
    {
        var criteria = AbstractTypeFactory<ChangeLogSearchCriteria>.TryCreateInstance();

        var types = AbstractTypeFactory<PickupLocation>.AllTypeInfos.Select(x => x.TypeName).ToList();

        if (types.Count != 0)
        {
            types.Add(nameof(PickupLocation));
            criteria.ObjectTypes = types;
        }
        else
        {
            criteria.ObjectType = nameof(PickupLocation);
        }

        criteria.StartDate = startDate;
        criteria.EndDate = endDate;
        criteria.Skip = (int)skip;
        criteria.Take = (int)take;

        return criteria;
    }
}
