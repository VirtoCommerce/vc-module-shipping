using System;
using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Services
{
    public interface IShippingMethodsRegistrar
    {
        void RegisterShippingMethod<T>(Func<T> factory = null) where T : ShippingMethod;
        Task<ShippingMethod[]> GetRegisteredMethods();
    }
}
