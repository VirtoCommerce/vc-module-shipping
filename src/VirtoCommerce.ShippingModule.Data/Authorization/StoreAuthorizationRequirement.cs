using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.ShippingModule.Data.Authorization;

public sealed class StoreAuthorizationRequirement(string permission) : PermissionAuthorizationRequirement(permission);
