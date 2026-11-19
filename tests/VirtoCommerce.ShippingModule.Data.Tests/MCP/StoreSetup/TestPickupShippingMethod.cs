namespace VirtoCommerce.ShippingModule.Data.Tests.MCP.StoreSetup;

// A second installed method, with settings of its own.
public sealed class TestPickupShippingMethod : FixedRateShippingMethod
{
    public const string PickupCode = "TestPickup";

    public TestPickupShippingMethod()
    {
        Code = PickupCode;
        Name = "Test pickup";
    }
}
