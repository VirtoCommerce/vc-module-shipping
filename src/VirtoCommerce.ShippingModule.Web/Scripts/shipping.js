//Call this to register our module to main application
var moduleName = "virtoCommerce.shippingModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['ngSanitize'])
    .run(['platformWebApp.widgetService',
        'platformWebApp.permissionScopeResolver',
        'virtoCommerce.storeModule.stores',
        'platformWebApp.bladeNavigationService',
        function (widgetService, scopeResolver, stores, bladeNavigationService) {

        widgetService.registerWidget({
            controller: 'virtoCommerce.shippingModule.storeShippingWidgetController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/widgets/storeShippingWidget.tpl.html'
        }, 'storeDetail');

        widgetService.registerWidget({
            controller: 'platformWebApp.entitySettingsWidgetController',
            template: '$(Platform)/Scripts/app/settings/widgets/entitySettingsWidget.tpl.html'
        }, 'shippingMethodDetail');

        widgetService.registerWidget({
            controller: 'virtoCommerce.shippingModule.pickupLocationsWidgetController',
            permission: 'pickup:read',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/widgets/pickupLocationsWidget.tpl.html',
            isVisible: function (blade) {
                return blade.shippingMethod?.typeName === 'BuyOnlinePickupInStoreShippingMethod';
            },
        }, 'shippingMethodDetail');

        widgetService.registerWidget({
            size: [2, 1],
            controller: 'virtoCommerce.shippingModule.pickupLocationsAddressWidgetController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/widgets/pickupLocationsAddressWidget.tpl.html'
        }, 'pickupLocationAddress');

        //Register permission scopes templates used for scope bounded definition in role management ui
        var selectedStoreScope = {
            type: 'SelectedStoreScope',
            title: 'Only for selected stores',
            selectFn: function (blade, callback) {
                var newBlade = {
                    id: 'store-pick',
                    title: this.title,
                    subtitle: 'Select stores',
                    currentEntity: this,
                    onChangesConfirmedFn: callback,
                    dataService: stores,
                    controller: 'platformWebApp.security.scopeValuePickFromSimpleListController',
                    template: '$(Platform)/Scripts/app/security/blades/common/scope-value-pick-from-simple-list.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }
        };
        scopeResolver.register(selectedStoreScope);
    }]);
