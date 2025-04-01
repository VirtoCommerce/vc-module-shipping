using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public interface IShippingRepository : IRepository
    {
        IQueryable<StoreShippingMethodEntity> ShippingMethods { get; }
        IQueryable<PickupLocationEntity> PickupLocations { get; }

        Task<IList<StoreShippingMethodEntity>> GetByIdsAsync(IList<string> ids);
        Task<IList<PickupLocationEntity>> GetPickupLocationsByIdsAsync(IList<string> ids);
    }
}
