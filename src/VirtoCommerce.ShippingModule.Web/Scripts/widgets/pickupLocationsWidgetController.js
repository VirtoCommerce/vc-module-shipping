angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationsWidgetController',
        ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "pickupLocationListBlade",
            storeId: blade.currentEntity.id, // ??
            title: 'shipping.widgets.store-shipping-widget.blade-title', // ??
            controller: 'virtoCommerce.shippingModule.pickupLocationListController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/pickupLocation-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);
