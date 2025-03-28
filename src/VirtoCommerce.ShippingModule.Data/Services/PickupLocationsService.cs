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

public class PickupLocationsService(
    Func<IPickupLocationsRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher
) : CrudService<PickupLocation, PickupLocationEntity, PickupLocationChangeEvent, PickupLocationChangedEvent>(repositoryFactory, platformMemoryCache, eventPublisher),
        IPickupLocationsService
{
    protected override async Task<IList<PickupLocationEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return await ((IPickupLocationsRepository)repository).GetPickupLocationsByIdsAsync(ids);
    }
}
