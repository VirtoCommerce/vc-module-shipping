namespace VirtoCommerce.ShippingModule.Core.Model.Search;

public class PickupLocationIndexedSearchCriteria : PickupLocationSearchCriteria
{
    public string Facet { get; set; }

    public string CountryCode { get; set; }
    public string RegionId { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
}
