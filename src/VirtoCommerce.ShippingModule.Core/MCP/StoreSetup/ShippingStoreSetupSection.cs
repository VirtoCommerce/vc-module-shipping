using System.Collections.Generic;
using System.ComponentModel;

namespace VirtoCommerce.ShippingModule.Core.MCP.StoreSetup;

// Shipping's section of a store's setup, as an agent fills it: "shipping": { "methods": [ { "code": ... } ] }.
// The descriptions become the JSON schema that get_store_setup_options shows.
public class ShippingStoreSetupSection
{
    [Description("The shipping methods the store offers: the complete list. A method that is not listed is switched off.")]
    public IList<ShippingStoreSetupMethod> Methods { get; set; }
}
