angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationListController',
        ['$scope', '$translate', 'platformWebApp.bladeNavigationService',
            'virtoCommerce.shippingModule.pickupLocations',
            function ($scope, $translate, bladeNavigationService, pickupLocations) {
                var blade = $scope.blade;

                function initializeBlade() {
                    blade.isLoading = false;
                    blade.headIcon = 'fa fa-truck';

                    blade.toolbarCommands = [
                        {
                            name: "platform.commands.refresh", icon: 'fa fa-refresh',
                            executeMethod: blade.refresh,
                            canExecuteMethod: function () { return true; }
                        }
                    ];

                    blade.refresh();
                };

                blade.refresh = function () {
                    blade.isLoading = true;
                    pickupLocations.getAll({ storeId: blade.storeId }, function (data) {
                        blade.isLoading = false;
                        blade.currentEntities = data;
                    }, function (error) {
                        bladeNavigationService.setError('Error ' + error.status, blade);
                    })
                    //pickupLocations.search({
                    //    storeId: blade.storeId
                    //}, function (data) {
                    //    blade.isLoading = false;

                    //    _.each(data.results, function (item) {
                    //        var nameTranslationKey = `shipping.labels.${item.typeName}.name`;
                    //        var descriptionTranslateKey = `shipping.labels.${item.typeName}.description`;

                    //        var nameResult = $translate.instant(nameTranslationKey);
                    //        var displayDescription = $translate.instant(descriptionTranslateKey);

                    //        item.displayName = nameResult === nameTranslationKey ? (item.name || item.typeName) : nameResult;
                    //        item.displayDescription = displayDescription === descriptionTranslateKey ? item.description : displayDescription;
                    //    });

                    //    blade.currentEntities = data.results;
                    //    blade.selectedShippingMethods = _.findWhere(blade.currentEntities, { isActive: true });
                    //}, function (error) {
                    //    bladeNavigationService.setError('Error ' + error.status, blade);
                    //});
                }

                $scope.selectNode = function (node) {
                    //$scope.selectedNodeId = node.typeName;
                    //var newBlade = {
                    //    id: 'shippingMethodDetail',
                    //    shippingMethod: node,
                    //    storeId: blade.storeId,
                    //    controller: 'virtoCommerce.shippingModule.shippingMethodDetailController',
                    //    template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/shippingMethod-detail.tpl.html'
                    //};
                    //bladeNavigationService.showBlade(newBlade, $scope.blade);
                };

                initializeBlade();

            }]);
