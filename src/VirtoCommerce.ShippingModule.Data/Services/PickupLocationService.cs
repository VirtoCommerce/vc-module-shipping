using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Events;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services;

public class PickupLocationService(
    Func<IShippingRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher
) : CrudService<PickupLocation, PickupLocationEntity, PickupLocationChangingEvent, PickupLocationChangedEvent>(repositoryFactory, platformMemoryCache, eventPublisher),
        IPickupLocationService
{
    protected override async Task<IList<PickupLocationEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return await ((IShippingRepository)repository).GetPickupLocationsByIdsAsync(ids);
    }
}
