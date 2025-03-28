using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ShippingModule.Core.Security;

/// <summary>
/// Restricts access rights to a particular store
/// </summary>
public sealed class StoreSelectedScope : PermissionScope
{
    public string StoreId => Scope;
}
