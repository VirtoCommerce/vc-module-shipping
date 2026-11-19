using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.MCP.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.MCP.StoreSetup;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.MCP.StoreSetup;
using VirtoCommerce.StoreModule.Core.MCP.StoreSetup;
using VirtoCommerce.StoreModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ShippingModule.Data.Tests.MCP.StoreSetup;

public class ShippingStoreSetupPartTests
{
    private const string StoreId = "eu-store";
    private const string FixedRateCode = "FixedRate";
    private const string PickupCode = TestPickupShippingMethod.PickupCode;

    private static readonly SettingDescriptor _rate = new() { Name = "Test.Pickup.Rate", ValueType = SettingValueType.Decimal, DefaultValue = 0m };
    private static readonly SettingDescriptor _mapsApiKey = new() { Name = "Test.Pickup.MapsApiKey", ValueType = SettingValueType.ShortText };
    private static readonly SettingDescriptor _pickupPoint = new() { Name = "Test.Pickup.PointId", ValueType = SettingValueType.ShortText, IsRequired = true };

    // The methods the store has saved; the search adds a new instance of every installed method the store has not saved.
    private readonly List<ShippingMethod> _saved = [];
    private readonly Mock<IShippingMethodsService> _methodsService = new();

    [Fact]
    public async Task GetChoicesAsync_ListsTheInstalledMethods_WithTheirSettingsThatAreNotSecrets()
    {
        //Act
        var choices = await CreatePart().GetChoicesAsync();

        //Assert
        Assert.Equal([FixedRateCode, PickupCode], choices.Select(x => x.Value));
        Assert.All(choices, x => Assert.Equal("shipping.methods.code", x.Field));
        Assert.Equal([_rate.Name, _pickupPoint.Name], choices.Single(x => x.Value == PickupCode).Settings.Select(x => x.Name));
    }

    [Fact]
    public async Task CheckAsync_WithoutAnActiveMethod_IsBlocking()
    {
        //Act
        var findings = await CreatePart().CheckAsync(CreateContext());

        //Assert
        var finding = Assert.Single(findings);
        Assert.Equal("shipping-no-active-method", finding.Id);
        Assert.Equal(McpFindingSeverities.Critical, finding.Severity);
    }

    [Fact]
    public async Task CheckAsync_WithACountry_SaysTheRatesAreDecidedAtCheckout()
    {
        //Arrange
        _saved.Add(new FixedRateShippingMethod { StoreId = StoreId, IsActive = true });
        var context = CreateContext();
        context.Country = "DEU";

        //Act
        var findings = await CreatePart().CheckAsync(context);

        //Assert
        var finding = Assert.Single(findings);
        Assert.Equal("shipping-country-not-checked", finding.Id);
        Assert.Equal(McpFindingSeverities.Info, finding.Severity);
        Assert.Contains("DEU", finding.Title);
        Assert.Contains(FixedRateCode, finding.Evidence);
    }

    [Fact]
    public async Task PlanAsync_TheListIsTheCompleteNewList_AndNothingIsSaved()
    {
        //Arrange
        _saved.Add(new FixedRateShippingMethod { StoreId = StoreId, IsActive = true });
        var section = CreateSection(new ShippingStoreSetupMethod { Code = PickupCode, Priority = 1, Settings = new Dictionary<string, object> { [_rate.Name] = 4.5m } });

        //Act
        var plan = await CreatePart().PlanAsync(CreateContext(), section);

        //Assert
        Assert.Empty(plan.Errors);
        Assert.Equal(
            [$"methods.{FixedRateCode}.active", $"methods.{PickupCode}.active", $"methods.{PickupCode}.priority", $"methods.{PickupCode}.{_rate.Name}"],
            plan.Changes.Select(x => x.Field));
        Assert.Contains(plan.Warnings, x => x.StartsWith("Switches off"));
        _methodsService.Verify(x => x.SaveChangesAsync(It.IsAny<IList<ShippingMethod>>()), Times.Never);
    }

    // A field that is left out changes nothing, as the agent is told; an empty list is what switches every method off.
    [Fact]
    public async Task PlanAsync_ASectionWithoutMethods_ChangesNothing()
    {
        //Arrange
        _saved.Add(new FixedRateShippingMethod { StoreId = StoreId, IsActive = true });

        //Act
        var withoutMethods = await CreatePart().PlanAsync(CreateContext(), new ShippingStoreSetupSection());
        var emptyList = await CreatePart().PlanAsync(CreateContext(), CreateSection());

        //Assert
        Assert.Empty(withoutMethods.Errors);
        Assert.Empty(withoutMethods.Changes);
        Assert.Equal([$"methods.{FixedRateCode}.active"], emptyList.Changes.Select(x => x.Field));
    }

    [Theory]
    [InlineData("unknown method", "'Unknown' is not installed")]
    [InlineData("secret setting", "is a secret")]
    public async Task PlanAsync_RefusesWhatTheMethodsCannotTake(string gap, string expectedError)
    {
        //Arrange
        var method = gap == "unknown method"
            ? new ShippingStoreSetupMethod { Code = "Unknown" }
            : new ShippingStoreSetupMethod { Code = PickupCode, Settings = new Dictionary<string, object> { [_mapsApiKey.Name] = "AIza-maps" } };

        //Act
        var plan = await CreatePart().PlanAsync(CreateContext(), CreateSection(method));

        //Assert
        Assert.Contains(plan.Errors, x => x.Contains(expectedError));
        Assert.DoesNotContain(plan.Errors, x => x.Contains("AIza-maps"));
    }

    // A setting the method never saved has its default: sending that value changes nothing.
    [Fact]
    public async Task PlanAsync_ASettingSentWithTheValueInEffect_ChangesNothing()
    {
        //Arrange
        _saved.Add(new TestPickupShippingMethod { StoreId = StoreId, IsActive = true });
        var section = CreateSection(new ShippingStoreSetupMethod { Code = PickupCode, Settings = new Dictionary<string, object> { [_rate.Name] = 0m } });

        //Act
        var plan = await CreatePart().PlanAsync(CreateContext(), section);

        //Assert
        Assert.Empty(plan.Errors);
        Assert.Empty(plan.Changes);
    }

    [Fact]
    public async Task ApplyAsync_SavesTheChangedMethodsForTheStore_AndARepeatChangesNothing()
    {
        //Arrange
        var part = CreatePart();
        var section = CreateSection(new ShippingStoreSetupMethod { Code = PickupCode, Settings = new Dictionary<string, object> { [_rate.Name] = 4.5m } });
        var context = CreateContext();
        var plan = await part.PlanAsync(context, section);

        //Act
        await part.ApplyAsync(context, plan);
        var repeat = await part.PlanAsync(CreateContext(), section);

        //Assert
        var saved = Assert.Single(_saved);
        Assert.Equal((PickupCode, StoreId, true), (saved.Code, saved.StoreId, saved.IsActive));
        Assert.Equal(4.5m, saved.Settings.Single(x => x.Name == _rate.Name).Value);
        Assert.Empty(repeat.Changes);
    }

    // Applying reads the methods again: a setting changed in the back office since the plan, which the plan does not
    // name, keeps its new value.
    [Fact]
    public async Task ApplyAsync_KeepsWhatChangedSinceThePlan()
    {
        //Arrange
        _saved.Add(new TestPickupShippingMethod { StoreId = StoreId, IsActive = true, Settings = [new ObjectSettingEntry(_rate) { Value = 1m }] });
        var part = CreatePart();
        var context = CreateContext();
        var plan = await part.PlanAsync(context, CreateSection(new ShippingStoreSetupMethod { Code = PickupCode, Settings = new Dictionary<string, object> { [_rate.Name] = 4.5m } }));
        _saved.Single().Settings.Add(new ObjectSettingEntry(_pickupPoint) { Value = "P-1" });

        //Act
        await part.ApplyAsync(context, plan);

        //Assert
        var saved = Assert.Single(_saved);
        Assert.Equal(4.5m, saved.Settings.Single(x => x.Name == _rate.Name).Value);
        Assert.Equal("P-1", saved.Settings.Single(x => x.Name == _pickupPoint.Name).Value);
    }

    // Q6: a required setting left empty blocks going live, as for payment methods.
    [Fact]
    public async Task CheckAsync_ActiveMethodWithAnEmptyRequiredSetting_IsBlocking()
    {
        //Arrange
        _saved.Add(new TestPickupShippingMethod { StoreId = StoreId, IsActive = true });

        //Act
        var findings = await CreatePart().CheckAsync(CreateContext());

        //Assert
        var finding = Assert.Single(findings);
        Assert.Equal(("shipping-method-needs-settings", McpFindingSeverities.Critical), (finding.Id, finding.Severity));
        Assert.Equal("Empty: Test.Pickup.PointId.", finding.Evidence);
        Assert.False(finding.ManualOnly);
        Assert.Equal(PickupCode, finding.ObjectId);
    }

    private static StoreSetupContext CreateContext()
    {
        return new StoreSetupContext { Store = new Store { Id = StoreId, Name = "EU store" } };
    }

    // The state lists the active methods, each with its settings in effect, and never a secret's value.
    [Fact]
    public async Task GetStateAsync_ListsTheActiveMethods_WithTheirSettings_WithoutSecrets()
    {
        //Arrange
        _saved.Add(new TestPickupShippingMethod
        {
            StoreId = StoreId,
            IsActive = true,
            Priority = 3,
            Settings = [new ObjectSettingEntry(_pickupPoint) { Value = "berlin-1" }, new ObjectSettingEntry(_mapsApiKey) { Value = "AIza-maps" }],
        });

        //Act
        var state = (ShippingStoreSetupSection)await CreatePart().GetStateAsync(CreateContext().Store);

        //Assert
        var method = Assert.Single(state.Methods);
        Assert.Equal((PickupCode, 3), (method.Code, method.Priority));
        Assert.Equal(new Dictionary<string, object> { [_rate.Name] = 0m, [_pickupPoint.Name] = "berlin-1" }, method.Settings);
        Assert.DoesNotContain("AIza-maps", JsonSerializer.Serialize(state));
    }

    [Fact]
    public async Task PlanAsync_AnEntryWithoutACode_IsRefused_AndACodeIsReadWithoutSpaces()
    {
        //Act
        var withoutCode = await CreatePart().PlanAsync(CreateContext(), CreateSection(new ShippingStoreSetupMethod { Code = PickupCode }, new ShippingStoreSetupMethod { Code = " " }));
        var spaced = await CreatePart().PlanAsync(CreateContext(), CreateSection(new ShippingStoreSetupMethod { Code = $" {PickupCode} " }));

        //Assert
        Assert.Equal(["Each shipping method needs its code. get_store_setup_options lists the codes."], withoutCode.Errors);
        Assert.Empty(spaced.Errors);
        Assert.Contains(spaced.Changes, x => x.Field == $"methods.{PickupCode}.active");
    }

    private static ShippingStoreSetupSection CreateSection(params ShippingStoreSetupMethod[] methods)
    {
        return new ShippingStoreSetupSection { Methods = methods };
    }

    private ShippingStoreSetupPart CreatePart()
    {
        var registrar = new Mock<IShippingMethodsRegistrar>();
        registrar.Setup(x => x.GetRegisteredMethods()).ReturnsAsync(() => [new FixedRateShippingMethod(), new TestPickupShippingMethod()]);

        var searchService = new Mock<IShippingMethodsSearchService>();
        searchService
            .Setup(x => x.SearchAsync(It.IsAny<ShippingMethodsSearchCriteria>(), It.IsAny<bool>()))
            .ReturnsAsync((ShippingMethodsSearchCriteria criteria, bool clone) =>
            {
                // As the search service: copies when asked, the saved instances themselves otherwise.
                var saved = _saved.Where(x => x.StoreId == criteria.StoreId).Select(x => clone ? x.CloneTyped() : x).ToList();
                var installed = new ShippingMethod[] { new FixedRateShippingMethod(), new TestPickupShippingMethod() };
                var all = saved.Concat(installed.Where(x => !saved.Any(s => s.Code == x.Code))).ToList();

                return new ShippingMethodsSearchResult { TotalCount = all.Count, Results = all.Skip(criteria.Skip).Take(criteria.Take).ToList() };
            });

        _methodsService
            .Setup(x => x.SaveChangesAsync(It.IsAny<IList<ShippingMethod>>()))
            .Callback((IList<ShippingMethod> methods) =>
            {
                foreach (var method in methods)
                {
                    _saved.RemoveAll(x => x.Code == method.Code && x.StoreId == method.StoreId);
                    _saved.Add(method.CloneTyped());
                }
            })
            .Returns(Task.CompletedTask);

        var settingsRegistrar = new Mock<ISettingsRegistrar>();
        settingsRegistrar.Setup(x => x.GetSettingsForType(nameof(TestPickupShippingMethod))).Returns([_rate, _mapsApiKey, _pickupPoint]);
        settingsRegistrar.Setup(x => x.GetSettingsForType(nameof(FixedRateShippingMethod))).Returns([]);

        return new ShippingStoreSetupPart(registrar.Object, searchService.Object, _methodsService.Object, settingsRegistrar.Object, new TestStoreSetupSettings());
    }
}
