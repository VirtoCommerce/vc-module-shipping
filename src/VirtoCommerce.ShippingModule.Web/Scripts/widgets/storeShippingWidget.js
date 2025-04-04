angular.module('virtoCommerce.shippingModule').controller('virtoCommerce.shippingModule.storeShippingWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            storeId: blade.currentEntity.id,
            title: 'shipping.widgets.store-shipping-widget.blade-title',
            controller: 'virtoCommerce.shippingModule.shippingMethodListController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/shippingMethod-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);
