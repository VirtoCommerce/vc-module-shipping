using System;
using System.Collections.Generic;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.StoreModule.Core.MCP.StoreSetup;

namespace VirtoCommerce.ShippingModule.Data.MCP.StoreSetup;

// The plan of the shipping section, carrying the store's methods as they will be after the change: the check reads them
// for the preview ("what the store would miss afterwards"), and applying saves the changed ones.
public class ShippingStoreSetupPlan : StoreSetupPartPlan
{
    public IList<ShippingMethod> Methods { get; set; } = [];

    public ISet<string> ChangedCodes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
