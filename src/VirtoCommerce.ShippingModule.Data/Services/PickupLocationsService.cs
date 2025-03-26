using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Events;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services;

public class PickupLocationsService(
    Func<IPickupLocationsRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher
) : CrudService<PickupLocation, PickupLocationEntity, PickupLocationChangeEvent, PickupLocationChangedEvent>(repositoryFactory, platformMemoryCache, eventPublisher),
        IPickupLocationsService
{
    protected override async Task<IList<PickupLocationEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return await ((IPickupLocationsRepository)repository).GetPickupLocationsByIdsAsync(ids);
    }

    //public Task<IEnumerable<Address>> GetAddresses()
    //{
    //    IEnumerable<Address> addresses = new List<Address>
    //    {
    //        new ()
    //        {
    //            Id = "92C3D5D5-06A7-4D78-85E9-94149296E412",
    //            Key = "ADDR-0001",
    //            Name = "CVS Pharmacy",
    //            Organization = "CVS Pharmacy",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Jose",
    //            PostalCode = "92472",
    //            Line1 = "1716 Washington Ave",
    //            Line2 = "",
    //            RegionId = "GA",
    //            RegionName = "Georgia",
    //            Phone = "+1-993-317-2293",
    //            Email = "contact@cvspharmacy.com",
    //            OuterId = "OUT-956836",
    //            IsDefault = true,
    //            Description = "Pickup point at CVS Pharmacy, San Jose, Georgia"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCDA",
    //            Key = "ADDR-0002",
    //            Name = "Shell Gas Station",
    //            Organization = "Shell Gas Station",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Jose",
    //            PostalCode = "47880",
    //            Line1 = "2314 Pine St",
    //            Line2 = "",
    //            RegionId = "HI",
    //            RegionName = "Hawaii",
    //            Phone = "+1-554-817-7179",
    //            Email = "contact@shellgasstation.com",
    //            OuterId = "OUT-573656",
    //            IsDefault = false,
    //            Description = "Pickup point at Shell Gas Station, San Jose, Hawaii"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD1",
    //            Key = "ADDR-0003",
    //            Name = "Best Buy",
    //            Organization = "Best Buy",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Los Angeles",
    //            PostalCode = "55892",
    //            Line1 = "6107 Elm St",
    //            Line2 = "",
    //            RegionId = "IA",
    //            RegionName = "Iowa",
    //            Phone = "+1-364-523-5472",
    //            Email = "contact@bestbuy.com",
    //            OuterId = "OUT-157720",
    //            IsDefault = false,
    //            Description = "Pickup point at Best Buy, Los Angeles, Iowa"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD2",
    //            Key = "ADDR-0004",
    //            Name = "Walgreens",
    //            Organization = "Walgreens",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Diego",
    //            PostalCode = "22698",
    //            Line1 = "2272 Cedar Ave",
    //            Line2 = "",
    //            RegionId = "DE",
    //            RegionName = "Delaware",
    //            Phone = "+1-862-302-5381",
    //            Email = "contact@walgreens.com",
    //            OuterId = "OUT-408135",
    //            IsDefault = true,
    //            Description = "Pickup point at Walgreens, San Diego, Delaware"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD3",
    //            Key = "ADDR-0005",
    //            Name = "Costco Wholesale",
    //            Organization = "Costco Wholesale",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "New York",
    //            PostalCode = "52117",
    //            Line1 = "1139 High St",
    //            Line2 = "",
    //            RegionId = "FL",
    //            RegionName = "Florida",
    //            Phone = "+1-607-337-7015",
    //            Email = "contact@costcowholesale.com",
    //            OuterId = "OUT-700367",
    //            IsDefault = true,
    //            Description = "Pickup point at Costco Wholesale, New York, Florida"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD4",
    //            Key = "ADDR-0006",
    //            Name = "7-Eleven",
    //            Organization = "7-Eleven",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Antonio",
    //            PostalCode = "72287",
    //            Line1 = "4918 High St",
    //            Line2 = "",
    //            RegionId = "LA",
    //            RegionName = "Louisiana",
    //            Phone = "+1-461-880-6874",
    //            Email = "contact@7-eleven.com",
    //            OuterId = "OUT-890746",
    //            IsDefault = false,
    //            Description = "Pickup point at 7-Eleven, San Antonio, Louisiana"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD5",
    //            Key = "ADDR-0007",
    //            Name = "Subway",
    //            Organization = "Subway",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Jose",
    //            PostalCode = "24838",
    //            Line1 = "4694 Elm St",
    //            Line2 = "",
    //            RegionId = "DE",
    //            RegionName = "Delaware",
    //            Phone = "+1-676-464-3962",
    //            Email = "contact@subway.com",
    //            OuterId = "OUT-415465",
    //            IsDefault = false,
    //            Description = "Pickup point at Subway, San Jose, Delaware"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD6",
    //            Key = "ADDR-0008",
    //            Name = "Starbucks",
    //            Organization = "Starbucks",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Phoenix",
    //            PostalCode = "44554",
    //            Line1 = "2462 Oak St",
    //            Line2 = "",
    //            RegionId = "AL",
    //            RegionName = "Alabama",
    //            Phone = "+1-304-113-3526",
    //            Email = "contact@starbucks.com",
    //            OuterId = "OUT-361927",
    //            IsDefault = false,
    //            Description = "Pickup point at Starbucks, Phoenix, Alabama"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD7",
    //            Key = "ADDR-0009",
    //            Name = "PetSmart",
    //            Organization = "PetSmart",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Antonio",
    //            PostalCode = "97449",
    //            Line1 = "499 Lake St",
    //            Line2 = "",
    //            RegionId = "LA",
    //            RegionName = "Louisiana",
    //            Phone = "+1-817-987-4028",
    //            Email = "contact@petsmart.com",
    //            OuterId = "OUT-573703",
    //            IsDefault = false,
    //            Description = "Pickup point at PetSmart, San Antonio, Louisiana"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD8",
    //            Key = "ADDR-0010",
    //            Name = "Office Depot",
    //            Organization = "Office Depot",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Antonio",
    //            PostalCode = "14849",
    //            Line1 = "6322 Maple Ave",
    //            Line2 = "",
    //            RegionId = "CO",
    //            RegionName = "Colorado",
    //            Phone = "+1-925-439-4528",
    //            Email = "contact@officedepot.com",
    //            OuterId = "OUT-839357",
    //            IsDefault = false,
    //            Description = "Pickup point at Office Depot, San Antonio, Colorado"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD9",
    //            Key = "ADDR-0011",
    //            Name = "Kroger",
    //            Organization = "Kroger",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "San Diego",
    //            PostalCode = "38583",
    //            Line1 = "5604 Main St",
    //            Line2 = "",
    //            RegionId = "AL",
    //            RegionName = "Alabama",
    //            Phone = "+1-478-799-6819",
    //            Email = "contact@kroger.com",
    //            OuterId = "OUT-108832",
    //            IsDefault = false,
    //            Description = "Pickup point at Kroger, San Diego, Alabama"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CCD0",
    //            Key = "ADDR-0012",
    //            Name = "Walmart Supercenter",
    //            Organization = "Walmart Supercenter",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Houston",
    //            PostalCode = "36804",
    //            Line1 = "8458 Cedar Ave",
    //            Line2 = "",
    //            RegionId = "CT",
    //            RegionName = "Connecticut",
    //            Phone = "+1-494-697-4374",
    //            Email = "contact@walmartsupercenter.com",
    //            OuterId = "OUT-807101",
    //            IsDefault = true,
    //            Description = "Pickup point at Walmart Supercenter, Houston, Connecticut"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC1A",
    //            Key = "ADDR-0013",
    //            Name = "Dollar Tree",
    //            Organization = "Dollar Tree",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Philadelphia",
    //            PostalCode = "61550",
    //            Line1 = "25 Broadway",
    //            Line2 = "",
    //            RegionId = "ME",
    //            RegionName = "Maine",
    //            Phone = "+1-908-991-7678",
    //            Email = "contact@dollartree.com",
    //            OuterId = "OUT-915598",
    //            IsDefault = true,
    //            Description = "Pickup point at Dollar Tree, Philadelphia, Maine"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC11",
    //            Key = "ADDR-0014",
    //            Name = "Home Depot",
    //            Organization = "Home Depot",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Houston",
    //            PostalCode = "10703",
    //            Line1 = "228 Main St",
    //            Line2 = "",
    //            RegionId = "MD",
    //            RegionName = "Maryland",
    //            Phone = "+1-350-555-5874",
    //            Email = "contact@homedepot.com",
    //            OuterId = "OUT-787945",
    //            IsDefault = true,
    //            Description = "Pickup point at Home Depot, Houston, Maryland"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC12",
    //            Key = "ADDR-0015",
    //            Name = "GameStop",
    //            Organization = "GameStop",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Chicago",
    //            PostalCode = "45678",
    //            Line1 = "1423 Main St",
    //            Line2 = "",
    //            RegionId = "IL",
    //            RegionName = "Illinois",
    //            Phone = "+1-213-555-1212",
    //            Email = "contact@gamestop.com",
    //            OuterId = "OUT-568912",
    //            IsDefault = false,
    //            Description = "Pickup point at GameStop, Chicago, Illinois"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC13",
    //            Key = "ADDR-0016",
    //            Name = "Trader Joe's",
    //            Organization = "Trader Joe's",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Los Angeles",
    //            PostalCode = "78901",
    //            Line1 = "333 Market St",
    //            Line2 = "",
    //            RegionId = "CA",
    //            RegionName = "California",
    //            Phone = "+1-310-777-8888",
    //            Email = "contact@traderjoes.com",
    //            OuterId = "OUT-672190",
    //            IsDefault = false,
    //            Description = "Pickup point at Trader Joe's, Los Angeles, California"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC14",
    //            Key = "ADDR-0017",
    //            Name = "IKEA",
    //            Organization = "IKEA",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Philadelphia",
    //            PostalCode = "19148",
    //            Line1 = "2206 S Columbus Blvd",
    //            Line2 = "",
    //            RegionId = "PA",
    //            RegionName = "Pennsylvania",
    //            Phone = "+1-215-551-4532",
    //            Email = "contact@ikea.com",
    //            OuterId = "OUT-239843",
    //            IsDefault = true,
    //            Description = "Pickup point at IKEA, Philadelphia, Pennsylvania"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC15",
    //            Key = "ADDR-0018",
    //            Name = "AutoZone",
    //            Organization = "AutoZone",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Dallas",
    //            PostalCode = "75201",
    //            Line1 = "1520 Main St",
    //            Line2 = "",
    //            RegionId = "TX",
    //            RegionName = "Texas",
    //            Phone = "+1-972-555-9090",
    //            Email = "contact@autozone.com",
    //            OuterId = "OUT-556298",
    //            IsDefault = false,
    //            Description = "Pickup point at AutoZone, Dallas, Texas"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC16",
    //            Key = "ADDR-0019",
    //            Name = "Whole Foods Market",
    //            Organization = "Whole Foods Market",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Austin",
    //            PostalCode = "73301",
    //            Line1 = "525 N Lamar Blvd",
    //            Line2 = "",
    //            RegionId = "TX",
    //            RegionName = "Texas",
    //            Phone = "+1-512-476-1206",
    //            Email = "contact@wholefoods.com",
    //            OuterId = "OUT-382918",
    //            IsDefault = true,
    //            Description = "Pickup point at Whole Foods Market, Austin, Texas"
    //        },
    //        new ()
    //        {
    //            Id = "F4A5E818-3E21-42DB-9577-1192E487CC17",
    //            Key = "ADDR-0020",
    //            Name = "Best Buy",
    //            Organization = "Best Buy",
    //            CountryCode = "US",
    //            CountryName = "United States",
    //            City = "Houston",
    //            PostalCode = "77002",
    //            Line1 = "5135 Richmond Ave",
    //            Line2 = "",
    //            RegionId = "TX",
    //            RegionName = "Texas",
    //            Phone = "+1-713-555-9876",
    //            Email = "contact@bestbuy.com",
    //            OuterId = "OUT-712983",
    //            IsDefault = false,
    //            Description = "Pickup point at Best Buy, Houston, Texas"
    //        }
    //    };
    //    return Task.FromResult(addresses);
    //}
}
