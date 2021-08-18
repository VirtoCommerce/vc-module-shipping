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
        public ShippingRepository(ShippingDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<StoreShippingMethodEntity> ShippingMethods => DbContext.Set<StoreShippingMethodEntity>();

        public Task<IEnumerable<StoreShippingMethodEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return Task.FromResult<IEnumerable<StoreShippingMethodEntity>>(ShippingMethods.Where(x => ids.Contains(x.Id)).ToList());
        }
    }
}
