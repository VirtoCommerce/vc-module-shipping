using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Events;

public class PickupLocationChangedEvent(IEnumerable<GenericChangedEntry<PickupLocation>> changedEntries)
    : GenericChangedEntryEvent<PickupLocation>(changedEntries);
