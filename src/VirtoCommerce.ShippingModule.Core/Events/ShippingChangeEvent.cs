using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Events;

public class ShippingChangeEvent(IEnumerable<GenericChangedEntry<ShippingMethod>> changedEntries)
    : GenericChangedEntryEvent<ShippingMethod>(changedEntries);
