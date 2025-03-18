using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model;

public class Address : ValueObject, IHasOuterId
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string Organization { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string RegionId { get; set; }
    public string RegionName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string OuterId { get; set; }
    public bool IsDefault { get; set; }
    public string Description { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        var result = base.GetEqualityComponents();
        if (!string.IsNullOrEmpty(Key))
        {
            result = new[] { Key };
        }
        return result;
    }

    public override string ToString()
    {
        return string.Join(", ", new[] { Line1, City, RegionName, PostalCode, CountryName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}

