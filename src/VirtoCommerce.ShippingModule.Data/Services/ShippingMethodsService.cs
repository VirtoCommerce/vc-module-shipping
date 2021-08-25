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
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.Events;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsService : CrudService<ShippingMethod, StoreShippingMethodEntity, ShippingChangeEvent, ShippingChangedEvent>, IShippingMethodsRegistrar, IShippingMethodsService
    {
        private readonly ISettingsManager _settingManager;

        public ShippingMethodsService(Func<IShippingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher, ISettingsManager settingManager)
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

        public Task<ShippingMethod[]> GetRegisteredMethods() =>
            Task.FromResult(
                AbstractTypeFactory<ShippingMethod>.AllTypeInfos
                .Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                .ToArray());

        protected override ShippingMethod ProcessModel(string responseGroup, StoreShippingMethodEntity entity, ShippingMethod model)
        {
            var shippingMethod = AbstractTypeFactory<ShippingMethod>.TryCreateInstance(string.IsNullOrEmpty(entity.TypeName) ? entity.Code : entity.TypeName);
            if (shippingMethod != null)
            {
                entity.ToModel(shippingMethod);
                _settingManager.DeepLoadSettingsAsync(shippingMethod).GetAwaiter().GetResult();
                return shippingMethod;
            }
            return null;
        }

        protected async override Task AfterSaveChangesAsync(IEnumerable<ShippingMethod> models, IEnumerable<GenericChangedEntry<ShippingMethod>> changedEntries)
        {
            await _settingManager.DeepSaveSettingsAsync(models);
        }


        protected async override Task<IEnumerable<StoreShippingMethodEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await ((IShippingRepository)repository).GetByIdsAsync(ids);
        }

        protected override async Task AfterDeleteAsync(IEnumerable<ShippingMethod> models, IEnumerable<GenericChangedEntry<ShippingMethod>> changedEntries)
        {
            await _settingManager.DeepRemoveSettingsAsync(models);
        }

        public async Task<ShippingMethod[]> GetByIdsAsync(string[] ids, string responseGroup)
        {
            var result = await base.GetByIdsAsync(ids);
            return result.ToArray();
        }

        public async Task SaveChangesAsync(ShippingMethod[] shippingMethods)
        {
            await base.SaveChangesAsync(shippingMethods);
        }

        public async Task DeleteAsync(string[] ids)
        {
            await base.DeleteAsync(ids);
        }
    }
}
