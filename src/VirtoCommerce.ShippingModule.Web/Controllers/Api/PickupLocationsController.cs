using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api;

[Route("api/shipping/pickup-locations")]
[Authorize]
public class PickupLocationsController(
    IPickupLocationsService pickupLocationsService,
    IPickupLocationsSearchService pickupLocationsSearchService
    ) : Controller
{
    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<PickupLocation>>> Search([FromBody] PickupLocationsSearchCriteria criteria)
    {
        var result = await pickupLocationsSearchService.SearchAsync(criteria);
        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<PickupLocation>> GetPickupLocationById(string id)
    {
        var result = await pickupLocationsService.GetNoCloneAsync(id);
        return Ok(result);
    }

    [HttpPut("")]
    public async Task<ActionResult<PickupLocation>> UpdatePickupLocation([FromBody] PickupLocation pickupLocation)
    {
        await pickupLocationsService.SaveChangesAsync([pickupLocation]);
        return Ok(pickupLocation);
    }
}
