using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core;
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
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<IEnumerable<PickupLocation>>> Search([FromBody] PickupLocationsSearchCriteria criteria)
    {
        // todo: validate storeId
        var result = await pickupLocationsSearchService.SearchAsync(criteria);
        return Ok(result);
    }

    [HttpPost]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult> CreatePickupLocation([FromBody] PickupLocation pickupLocation)
    {
        // todo: validate storeId
        await pickupLocationsService.SaveChangesAsync([pickupLocation]);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<PickupLocation>> GetPickupLocationById(string id)
    {
        // todo: validate storeId
        var result = await pickupLocationsService.GetNoCloneAsync(id);
        return Ok(result);
    }

    [HttpPut("")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    public async Task<ActionResult<PickupLocation>> UpdatePickupLocation([FromBody] PickupLocation pickupLocation)
    {
        // todo: validate storeId
        await pickupLocationsService.SaveChangesAsync([pickupLocation]);
        return Ok(pickupLocation);
    }

    [HttpDelete("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    public async Task<ActionResult> DeletePickupLocation(string id)
    {
        // todo: validate storeId
        await pickupLocationsService.DeleteAsync([id]);
        return Ok();
    }
}
