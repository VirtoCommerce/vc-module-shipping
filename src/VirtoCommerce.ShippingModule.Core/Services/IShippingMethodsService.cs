using System;
using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Services
{
    /// <summary>
    /// This interface should implement <see cref="ICrudService<ShippingMethod>"/> without methods.
    /// Methods left for compatibility and should be removed after upgrade to inheritance
    /// </summary>
    public interface IShippingMethodsService
    {
        [Obsolete(@"Need to remove after inherit IShippingMethodsService from ICrudService<ShippingMethod>")]
        Task<ShippingMethod[]> GetByIdsAsync(string[] ids, string responseGroup);
        [Obsolete(@"Need to remove after inherit IShippingMethodsService from ICrudService<ShippingMethod>")]
        Task<ShippingMethod> GetByIdAsync(string id, string responseGroup);
        [Obsolete(@"Need to remove after inherit IShippingMethodsService from ICrudService<ShippingMethod>")]
        Task SaveChangesAsync(ShippingMethod[] shippingMethods);
        [Obsolete(@"Need to remove after inherit IShippingMethodsService from ICrudService<ShippingMethod>")]
        Task DeleteAsync(string[] ids);
    }
}
