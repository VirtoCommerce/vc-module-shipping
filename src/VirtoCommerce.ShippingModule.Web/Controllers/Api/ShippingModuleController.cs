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

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api
{
    [Route("api/shipping")]
    [Authorize]
    public class ShippingModuleController(
        IShippingMethodsSearchService shippingMethodsSearchService,
        IShippingMethodsService shippingMethodsService,
        IShippingMethodsRegistrar shippingMethodsRegistrar,
        IAuthorizationService authorizationService)
        : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IList<ShippingMethod>>> GetRegisteredShippingMethods()
        {
            var result = await shippingMethodsRegistrar.GetRegisteredMethods();
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<ActionResult<ShippingMethodsSearchResult>> SearchShippingMethods([FromBody] ShippingMethodsSearchCriteria criteria)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, criteria,
                new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await shippingMethodsSearchService.SearchNoCloneAsync(criteria);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethodById(string id)
        {
            var result = await shippingMethodsService.GetNoCloneAsync(id);
            return Ok(result);
        }

        [HttpPut("")]
        public async Task<ActionResult<ShippingMethod>> UpdateShippingMethod([FromBody] ShippingMethod shippingMethod)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, shippingMethod,
                new StoreAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            await shippingMethodsService.SaveChangesAsync([shippingMethod]);
            return Ok(shippingMethod);
        }
    }
}
