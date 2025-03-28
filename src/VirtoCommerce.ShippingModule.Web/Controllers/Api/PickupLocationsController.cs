using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Authorization;

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api;

[Route("api/shipping/pickup-locations")]
[Authorize]
public class PickupLocationsController(
    IPickupLocationsService pickupLocationsService,
    IPickupLocationsSearchService pickupLocationsSearchService,
    IAuthorizationService authorizationService
    ) : Controller
{
    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<PickupLocation>>> Search([FromBody] PickupLocationsSearchCriteria criteria)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, criteria,
            new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await pickupLocationsSearchService.SearchAsync(criteria);
        return Ok(result);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreatePickupLocation([FromBody] PickupLocation pickupLocation)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, pickupLocation,
            new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Create));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await pickupLocationsService.SaveChangesAsync([pickupLocation]);
        return Ok();
    }

    [HttpGet]
    [Route("{storeId}/{id}")]
    public async Task<ActionResult<PickupLocation>> GetPickupLocationById([FromRoute] string id, [FromRoute] string storeId)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, storeId,
            new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await pickupLocationsService.GetNoCloneAsync(id);
        return Ok(result);
    }

    [HttpPut("")]
    public async Task<ActionResult<PickupLocation>> UpdatePickupLocation([FromBody] PickupLocation pickupLocation)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, pickupLocation,
            new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await pickupLocationsService.SaveChangesAsync([pickupLocation]);
        return Ok(pickupLocation);
    }

    [HttpDelete("{storeId}/{id}")]
    public async Task<ActionResult> DeletePickupLocation([FromRoute] string id, [FromRoute] string storeId)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, storeId,
            new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Delete));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await pickupLocationsService.DeleteAsync([id]);
        return Ok();
    }
}
