using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Security;
using VirtoCommerce.ShippingModule.Data.Authorization;

namespace VirtoCommerce.ShippingModule.Web.Authorization;

public sealed class StoreAuthorizationHandler(IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
    : PermissionAuthorizationHandlerBase<StoreAuthorizationRequirement>
{
    private readonly MvcNewtonsoftJsonOptions _jsonOptions = jsonOptions.Value;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        StoreAuthorizationRequirement requirement)
    {
        await base.HandleRequirementAsync(context, requirement);
        if (!context.HasSucceeded)
        {
            var userPermission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);
            if (userPermission != null)
            {
                var storeSelectedScopes = userPermission.AssignedScopes.OfType<StoreSelectedScope>();
                var allowedStoreIds = storeSelectedScopes.Select(x => x.StoreId).Distinct().ToArray();
                if (context.Resource is PickupLocationsSearchCriteria criteria)
                {
                    if (!criteria.ObjectIds.IsNullOrEmpty())
                    {
                        var scopedObjectIds = criteria.ObjectIds.Intersect(allowedStoreIds).ToArray();
                        if (scopedObjectIds.Length == 0)
                        {
                            context.Fail();
                        }
                        else
                        {
                            criteria.ObjectIds = scopedObjectIds;
                            context.Succeed(requirement);
                        }
                    }
                    else if (criteria.StoreId != null && allowedStoreIds.Contains(criteria.StoreId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else if (context.Resource is PickupLocation pickupLocation && allowedStoreIds.Contains(pickupLocation.StoreId))
                {
                    context.Succeed(requirement);
                }
                else if (context.Resource is string storeId && allowedStoreIds.Contains(storeId))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
