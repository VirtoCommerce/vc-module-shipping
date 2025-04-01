using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public class ShippingRepository(ShippingDbContext dbContext)
        : DbContextRepositoryBase<ShippingDbContext>(dbContext), IShippingRepository
    {
        public IQueryable<StoreShippingMethodEntity> ShippingMethods => DbContext.Set<StoreShippingMethodEntity>();
        public IQueryable<PickupLocationEntity> PickupLocations => DbContext.Set<PickupLocationEntity>();

        public async Task<IList<StoreShippingMethodEntity>> GetByIdsAsync(IList<string> ids)
        {
            return await ShippingMethods
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<IList<PickupLocationEntity>> GetPickupLocationsByIdsAsync(IList<string> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return [];
            }

            return await PickupLocations
                .Include(x => x.TransferFulfillmentCenters)
                .Where(x => ids.Contains(x.Id))
                .AsSplitQuery()
                .ToArrayAsync();
        }
    }
}
