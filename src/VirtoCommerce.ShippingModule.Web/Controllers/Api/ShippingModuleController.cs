using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api
{
    [Route("api/shipping")]
    [Authorize]
    public class ShippingModuleController : Controller
    {
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IShippingMethodsService _shippingMethodsService;
        private readonly IShippingMethodsRegistrar _shippingMethodsRegistrar;

        public ShippingModuleController(
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShippingMethodsService shippingMethodsService,
            IShippingMethodsRegistrar shippingMethodsRegistrar
            )
        {
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _shippingMethodsService = shippingMethodsService;
            _shippingMethodsRegistrar = shippingMethodsRegistrar;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<ShippingMethod>>> GetRegisteredShippingMethods()
        {
            var result = await _shippingMethodsRegistrar.GetRegisteredMethods();
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<ActionResult<ShippingMethodsSearchResult>> SearchShippingMethods([FromBody] ShippingMethodsSearchCriteria criteria)
        {
            var result = await _shippingMethodsSearchService.SearchNoCloneAsync(criteria);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethodById(string id)
        {
            var result = await _shippingMethodsService.GetNoCloneAsync(id);
            return Ok(result);
        }

        [HttpPut("")]
        public async Task<ActionResult<ShippingMethod>> UpdateShippingMethod([FromBody] ShippingMethod shippingMethod)
        {
            await _shippingMethodsService.SaveChangesAsync(new[] { shippingMethod });
            return Ok(shippingMethod);
        }
    }
}
