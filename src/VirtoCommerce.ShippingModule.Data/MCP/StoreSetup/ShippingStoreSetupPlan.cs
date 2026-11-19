using System.Collections.Generic;
using VirtoCommerce.MCP.Core.Models;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.MCP.StoreSetup;

// The plan of the shipping section, carrying the store's methods as they will be after the change: the check reads them
// for the preview ("what the store would miss afterwards"), and applying copies the fields of the plan's rows from them.
public class ShippingStoreSetupPlan : McpSetupPartPlan
{
    public IList<ShippingMethod> Methods { get; set; } = [];
}
