using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ShippingModule.Core.Security;

/// <summary>
/// Restricts access rights to a particular store
/// </summary>
public sealed class SelectedStoreScope : PermissionScope
{
    public string StoreId => Scope;
}
