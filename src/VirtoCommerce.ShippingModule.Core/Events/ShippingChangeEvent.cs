using System.Collections.Generic;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ShippingModule.Core.Events
{
    public class ShippingChangeEvent : GenericChangedEntryEvent<ShippingMethod>
    {
        public ShippingChangeEvent(IEnumerable<GenericChangedEntry<ShippingMethod>> changedEntries) : base(changedEntries)
        {
        }
    }
}
