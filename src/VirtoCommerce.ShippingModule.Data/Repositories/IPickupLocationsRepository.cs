using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories;

public interface IPickupLocationsRepository : IRepository
{
    IQueryable<PickupLocationEntity> PickupLocations { get; }

    Task<IList<PickupLocationEntity>> GetPickupLocationsByIdsAsync(IList<string> ids);
}
