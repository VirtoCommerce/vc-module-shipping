using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MCP.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.MCP.StoreSetup;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Security;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.MCP.StoreSetup;
using VirtoCommerce.StoreModule.Core.Model;
using ShippingPermissions = VirtoCommerce.ShippingModule.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.ShippingModule.Data.MCP.StoreSetup;

// Shipping's part of a store's setup: the shipping methods a store offers, with their settings that are not secrets.
// Store never references this class: Shipping registers it in its Module.Initialize with AddStoreSetupPart.
public class ShippingStoreSetupPart(
    IShippingMethodsRegistrar shippingMethodsRegistrar,
    IShippingMethodsSearchService shippingMethodsSearchService,
    IShippingMethodsService shippingMethodsService,
    ISettingsRegistrar settingsRegistrar,
    IStoreSetupSettings setupSettings)
    : IStoreSetupChoices, IStoreSetupState, IStoreSetupCheck, IStoreSetupSection
{
    public const string PartName = "shipping";

    public virtual string Name => PartName;

    public virtual string Title => "Shipping methods";

    // Shipping's own permissions, limited to stores by its SelectedStoreScope. Setting shipping up on a new store changes
    // the store's methods, so it needs shipping:update too.
    public virtual McpSetupPermissions Permissions => new()
    {
        Read = ShippingPermissions.Read,
        Update = ShippingPermissions.Update,
        Create = ShippingPermissions.Update,
        ScopeType = nameof(SelectedStoreScope),
    };

    public virtual Type SectionType => typeof(ShippingStoreSetupSection);

    // A store can be created without shipping; the readiness check then says it cannot go live.
    public virtual bool IsRequiredForNewStore => false;

    // 1. What can be chosen: every shipping method installed on the platform, with the settings it takes.
    public virtual async Task<IList<McpSetupChoice>> GetChoicesAsync()
    {
        var choices = new List<McpSetupChoice>();

        foreach (var method in await shippingMethodsRegistrar.GetRegisteredMethods())
        {
            var choice = AbstractTypeFactory<McpSetupChoice>.TryCreateInstance();
            choice.Field = $"{PartName}.methods.code";
            choice.Value = method.Code;
            choice.Title = method.Name;
            choice.Description = method.Description;

            // Secrets are not even listed, so the agent never proposes to send one.
            choice.Settings = setupSettings.DescribeAll(GetSettingDescriptors(method));

            choices.Add(choice);
        }

        return choices;
    }

    // 2. The store's current state, in the shape of the section: its active methods, by priority.
    public virtual async Task<object> GetStateAsync(Store store)
    {
        var methods = await GetStoreMethodsAsync(store.Id);

        var section = AbstractTypeFactory<ShippingStoreSetupSection>.TryCreateInstance();
        section.Methods = methods
            .Where(x => x.IsActive)
            .OrderBy(x => x.Priority)
            .Select(ToSectionMethod)
            .ToList();

        return section;
    }

    // 3. What is missing. When the request plans a shipping section, the check reads the methods as they will be after
    // the change (the plan), so the preview says what the store would miss afterwards. Otherwise it reads the saved ones.
    public virtual async Task<IList<McpFinding>> CheckAsync(StoreSetupContext context)
    {
        var store = context.Store;
        var methods = context.Plans.TryGetValue(PartName, out var plan) && plan is ShippingStoreSetupPlan shippingPlan
            ? shippingPlan.Methods
            : await GetStoreMethodsAsync(store.Id);

        var activeMethods = methods.Where(x => x.IsActive).OrderBy(x => x.Priority).ToList();
        var findings = new List<McpFinding>();

        if (activeMethods.Count == 0)
        {
            findings.Add(CreateFinding(store, "shipping-no-active-method", McpFindingSeverities.Critical,
                "No shipping method is active",
                "Customers cannot choose how their order is delivered, so they cannot check out.",
                $"0 of {methods.Count} installed shipping methods are active.",
                "Activate at least one shipping method."));
        }

        // Nothing in a store limits the countries it ships to; the methods decide their rates at checkout.
        if (!string.IsNullOrEmpty(context.Country))
        {
            findings.Add(CreateFinding(store, "shipping-country-not-checked", McpFindingSeverities.Info,
                $"Shipping to {context.Country} is not checked here",
                "Nothing here blocks it: each shipping method decides its rates for an address at checkout.",
                $"Active shipping methods: {(activeMethods.Count == 0 ? "none" : string.Join(", ", activeMethods.Select(x => x.Code)))}.",
                "Check the rates of the active methods for this country in the back office."));
        }

        return findings;
    }

    // 4a. Plans the section without side effects: the search returns copies, and nothing is saved here.
    public virtual async Task<McpSetupPartPlan> PlanAsync(StoreSetupContext context, object section)
    {
        var requested = ((ShippingStoreSetupSection)section).Methods ?? [];

        var plan = AbstractTypeFactory<ShippingStoreSetupPlan>.TryCreateInstance();
        plan.Methods = await GetStoreMethodsAsync(context.Store.Id);

        foreach (var code in requested.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1).Select(x => x.Key))
        {
            plan.Errors.Add($"Shipping method '{code}' is listed twice.");
        }

        foreach (var item in requested.Where(x => !plan.Methods.Any(method => method.Code.EqualsIgnoreCase(x.Code))))
        {
            plan.Errors.Add($"Shipping method '{item.Code}' is not installed. get_store_setup_options lists the codes.");
        }

        // Any error refuses the whole request, in every part, before anything is applied.
        if (plan.Errors.Count > 0)
        {
            return plan;
        }

        // The list is the complete new list: a listed method is active, any other one is switched off.
        foreach (var method in plan.Methods)
        {
            var item = requested.FirstOrDefault(x => x.Code.EqualsIgnoreCase(method.Code));

            PlanActive(plan, method, isActive: item != null);

            if (item != null)
            {
                PlanPriority(plan, method, item.Priority);
                PlanSettings(plan, method, item.Settings);
            }
        }

        return plan;
    }

    // 4b. Saves the methods the plan changes. The plan was made from the saved rows a moment ago, so a repeat after a
    // failure finds the applied methods unchanged and saves only the rest.
    public virtual async Task ApplyAsync(StoreSetupContext context, McpSetupPartPlan plan)
    {
        var shippingPlan = (ShippingStoreSetupPlan)plan;
        var changed = shippingPlan.Methods.Where(x => shippingPlan.ChangedCodes.Contains(x.Code)).ToList();

        if (changed.Count == 0)
        {
            return;
        }

        // A method the store has never saved comes from the search without a store.
        foreach (var method in changed)
        {
            method.StoreId = context.Store.Id;
        }

        await shippingMethodsService.SaveChangesAsync(changed);
    }

    protected virtual void PlanActive(ShippingStoreSetupPlan plan, ShippingMethod method, bool isActive)
    {
        if (method.IsActive == isActive)
        {
            return;
        }

        AddChange(plan, method, "active", method.IsActive, isActive);

        if (!isActive)
        {
            plan.Warnings.Add($"Switches off {method.Name}: customers can no longer choose it.");
        }

        method.IsActive = isActive;
    }

    protected virtual void PlanPriority(ShippingStoreSetupPlan plan, ShippingMethod method, int? priority)
    {
        if (priority == null || priority == method.Priority)
        {
            return;
        }

        AddChange(plan, method, "priority", method.Priority, priority.Value);
        method.Priority = priority.Value;
    }

    protected virtual void PlanSettings(ShippingStoreSetupPlan plan, ShippingMethod method, IDictionary<string, object> settings)
    {
        // Store's rules for a settings map, the same in every part: an unknown name, a secret or a wrong value is
        // refused, a null returns the default, and a value equal to the one in effect changes nothing.
        var changed = setupSettings.PlanSettings(plan, method, GetSettingDescriptors(method), settings,
            $"methods.{method.Code}.", $"Shipping method '{method.Code}'", "get_store_setup_options");

        if (changed.Count > 0)
        {
            plan.ChangedCodes.Add(method.Code);
        }
    }

    // The store's saved methods, plus a new instance of every installed method the store has not saved yet.
    protected virtual async Task<IList<ShippingMethod>> GetStoreMethodsAsync(string storeId)
    {
        var criteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
        criteria.StoreId = storeId;

        return await shippingMethodsSearchService.SearchAllAsync(criteria);
    }

    // The settings a method takes: each shipping module registers them for its method's type name.
    protected virtual IList<SettingDescriptor> GetSettingDescriptors(ShippingMethod method)
    {
        return settingsRegistrar.GetSettingsForType(method.TypeName)?.ToList() ?? [];
    }

    protected virtual ShippingStoreSetupMethod ToSectionMethod(ShippingMethod method)
    {
        var item = AbstractTypeFactory<ShippingStoreSetupMethod>.TryCreateInstance();
        item.Code = method.Code;
        item.Priority = method.Priority;
        item.Settings = setupSettings.GetValues(method, GetSettingDescriptors(method));

        return item;
    }

    protected virtual McpFinding CreateFinding(Store store, string id, string severity, string title, string why, string evidence, string fix)
    {
        var finding = StoreSetupFindings.ForStore(store, PartName, id, severity, "Shipping methods");
        finding.Title = title;
        finding.Why = why;
        finding.Evidence = evidence;
        finding.Fix = fix;

        return finding;
    }

    private static void AddChange(ShippingStoreSetupPlan plan, ShippingMethod method, string field, object from, object to)
    {
        plan.AddChange($"methods.{method.Code}.{field}", from, to);
        plan.ChangedCodes.Add(method.Code);
    }
}
