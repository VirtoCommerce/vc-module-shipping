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

namespace VirtoCommerce.ShippingModule.Data.Authorization;

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
                var storeSelectedScopes = userPermission.AssignedScopes.OfType<SelectedStoreScope>();
                var allowedStoreIds = storeSelectedScopes.Select(x => x.Scope).Distinct().ToArray();
                if (context.Resource is ShippingMethodsSearchCriteria shippingMethodsSearchCriteria)
                {
                    if (!shippingMethodsSearchCriteria.ObjectIds.IsNullOrEmpty())
                    {
                        var scopedObjectIds = shippingMethodsSearchCriteria.ObjectIds.Intersect(allowedStoreIds).ToArray();
                        if (scopedObjectIds.Length == 0)
                        {
                            context.Fail();
                        }
                        else
                        {
                            shippingMethodsSearchCriteria.ObjectIds = scopedObjectIds;
                            context.Succeed(requirement);
                        }
                    }
                    else if (shippingMethodsSearchCriteria.StoreId != null && allowedStoreIds.Contains(shippingMethodsSearchCriteria.StoreId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else if (context.Resource is PickupLocationSearchCriteria pickupLocationSearchCriteria)
                {
                    if (!pickupLocationSearchCriteria.ObjectIds.IsNullOrEmpty())
                    {
                        var scopedObjectIds = pickupLocationSearchCriteria.ObjectIds.Intersect(allowedStoreIds).ToArray();
                        if (scopedObjectIds.Length == 0)
                        {
                            context.Fail();
                        }
                        else
                        {
                            pickupLocationSearchCriteria.ObjectIds = scopedObjectIds;
                            context.Succeed(requirement);
                        }
                    }
                    else if (pickupLocationSearchCriteria.StoreId != null && allowedStoreIds.Contains(pickupLocationSearchCriteria.StoreId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else if (context.Resource is PickupLocation pickupLocation && allowedStoreIds.Contains(pickupLocation.StoreId))
                {
                    context.Succeed(requirement);
                }
                else if (context.Resource is ShippingMethod shippingMethod && allowedStoreIds.Contains(shippingMethod.StoreId))
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
