using System.Collections.Generic;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ShippingModule.Core.Events
{
    public class ShippingChangedEvent : GenericChangedEntryEvent<ShippingMethod>
    {
        public ShippingChangedEvent(IEnumerable<GenericChangedEntry<ShippingMethod>> changedEntries) : base(changedEntries)
        {
        }
    }
}
