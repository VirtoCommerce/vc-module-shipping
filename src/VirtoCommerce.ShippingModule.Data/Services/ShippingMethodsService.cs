using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Events;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsService : CrudService<ShippingMethod, StoreShippingMethodEntity, ShippingChangeEvent, ShippingChangedEvent>, IShippingMethodsRegistrar, IShippingMethodsService
    {
        private readonly ISettingsManager _settingManager;

        public ShippingMethodsService(
            Func<IShippingRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            ISettingsManager settingManager)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _settingManager = settingManager;
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

        public Task<ShippingMethod[]> GetRegisteredMethods()
        {
            var result = AbstractTypeFactory<ShippingMethod>.AllTypeInfos
                .Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                .ToArray();

            return Task.FromResult(result);
        }


        protected override ShippingMethod ProcessModel(string responseGroup, StoreShippingMethodEntity entity, ShippingMethod model)
        {
            var shippingMethod = AbstractTypeFactory<ShippingMethod>.TryCreateInstance(string.IsNullOrEmpty(entity.TypeName) ? $"{entity.Code}ShippingMethod" : entity.TypeName);
            if (shippingMethod != null)
            {
                entity.ToModel(shippingMethod);
                _settingManager.DeepLoadSettingsAsync(shippingMethod).GetAwaiter().GetResult();
                return shippingMethod;
            }
            return null;
        }

        protected override Task AfterSaveChangesAsync(IList<ShippingMethod> models, IList<GenericChangedEntry<ShippingMethod>> changedEntries)
        {
            return _settingManager.DeepSaveSettingsAsync(models);
        }

        protected override Task<IList<StoreShippingMethodEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((IShippingRepository)repository).GetByIdsAsync(ids);
        }

        protected override Task AfterDeleteAsync(IList<ShippingMethod> models, IList<GenericChangedEntry<ShippingMethod>> changedEntries)
        {
            return _settingManager.DeepRemoveSettingsAsync(models);
        }
    }
}
