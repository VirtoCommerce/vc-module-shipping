using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public class ShippingRepository : DbContextRepositoryBase<ShippingDbContext>, IShippingRepository
    {
        public ShippingRepository(ShippingDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<StoreShippingMethodEntity> ShippingMethods => DbContext.Set<StoreShippingMethodEntity>();

        public async Task<IList<StoreShippingMethodEntity>> GetByIdsAsync(IList<string> ids)
        {
            return await ShippingMethods
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }
    }
}
