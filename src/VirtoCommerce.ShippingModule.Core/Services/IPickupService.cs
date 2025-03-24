using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Services;

public interface IPickupService
{
    public Task<IEnumerable<Address>> GetAddresses();
}
