using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api
{
    [Route("api/shipping")]
    [Authorize]
    public class ShippingModuleController : Controller
    {
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly ICrudService<ShippingMethod> _shippingMethodsService;
        private readonly IShippingMethodsRegistrar _shippingMethodsRegistrar;

        public ShippingModuleController(
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShippingMethodsService shippingMethodsService,
            IShippingMethodsRegistrar shippingMethodsRegistrar
            )
        {
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _shippingMethodsService = (ICrudService<ShippingMethod>)shippingMethodsService;
            _shippingMethodsRegistrar = shippingMethodsRegistrar;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<ShippingMethod>> GetRegisteredShippingMethods()
        {
            var result = await _shippingMethodsRegistrar.GetRegisteredMethods();
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<ActionResult<ShippingMethodsSearchResult>> SearchShippingMethods([FromBody] ShippingMethodsSearchCriteria criteria)
        {
            var result = await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethodById(string id)
        {
            var result = await _shippingMethodsService.GetByIdAsync(id, null);
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
