using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories;

public class PickupLocationsRepository(ShippingDbContext db) : DbContextRepositoryBase<ShippingDbContext>(db),
    IPickupLocationsRepository
{
    public IQueryable<PickupLocationEntity> PickupLocations => DbContext.Set<PickupLocationEntity>();

    public async Task<IList<PickupLocationEntity>> GetPickupLocationsByIdsAsync(IList<string> ids)
    {
        if (ids.IsNullOrEmpty())
        {
            return [];
        }

        var result = await PickupLocations.Where(x => ids.Contains(x.Id)).ToArrayAsync();

        return !result.Any() ? [] : result;
    }
}
