using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Events;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsService : CrudService<ShippingMethod, StoreShippingMethodEntity, ShippingChangeEvent, ShippingChangedEvent>, IShippingMethodsRegistrar, IShippingMethodsService
    {
        public ShippingMethodsService(Func<IShippingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        public void RegisterShippingMethod<T>(Func<T> factory = null) where T : ShippingMethod
        {
            if (AbstractTypeFactory<ShippingMethod>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                var typeInfo = AbstractTypeFactory<ShippingMethod>.RegisterType<T>();
                if (factory != null)
                {
                    typeInfo.WithFactory(factory);
                }
            }
        }

        public Task<ShippingMethod[]> GetRegisteredMethods() =>
            Task.FromResult(
                AbstractTypeFactory<ShippingMethod>.AllTypeInfos
                .Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                .ToArray());

        protected override Task<IEnumerable<StoreShippingMethodEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return ((IShippingRepository)repository).GetByIdsAsync(ids);
        }

    }
}
