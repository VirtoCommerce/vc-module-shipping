angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationsWidgetController',
        ['$scope', 'platformWebApp.bladeNavigationService',
            'virtoCommerce.shippingModule.pickupLocations',
            function ($scope, bladeNavigationService, pickupLocations) {
                var blade = $scope.blade;

                $scope.totalCount = 'N/A';

                function refreshCount() {
                    pickupLocations.search({
                        storeId: blade.storeId,
                        take: 0,
                    }, function (data) {
                        $scope.totalCount = data.totalCount;
                    }, function (error) {
                        bladeNavigationService.setError('Error ' + error.status, blade);
                        $scope.totalCount = 'N/A';
                    });
                }

                refreshCount();

                $scope.openBlade = function () {
                    var newBlade = {
                        id: "pickupLocationListBlade",
                        storeId: blade.storeId, // ??
                        refreshWidget: function () { refreshCount(); },
                        title: 'shipping.widgets.pickup-location-widget.blade-title', // ??
                        controller: 'virtoCommerce.shippingModule.pickupLocationListController',
                        template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/pickupLocation-list.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                };
            }]);
