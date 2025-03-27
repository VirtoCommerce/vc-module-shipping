angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationListController',
        ['$scope', '$translate', 'platformWebApp.bladeNavigationService',
            'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper',
            'virtoCommerce.shippingModule.pickupLocations',
            function ($scope, $translate, bladeNavigationService, bladeUtils, uiGridConstants, uiGridHelper, pickupLocations) {
                $scope.uiGridConstants = uiGridConstants;

                var blade = $scope.blade;
                blade.isLoading = false;

                // function initializeBlade() {
                    
                    blade.headIcon = 'fa fa-truck';

                    blade.toolbarCommands = [
                        {
                            name: "platform.commands.refresh", icon: 'fa fa-refresh',
                            executeMethod: function () {
                                blade.refresh();
                            },
                            canExecuteMethod: function () { return true; }
                        },
                        {
                            name: "platform.commands.add", icon: 'fas fa-plus',
                            executeMethod: function () {
                                showDetailBlade();
                            },
                            canExecuteMethod: function () {
                                return true;
                            },
                        }
                    ];

                    // blade.refresh();
                // };

                blade.refresh = function () {
                    blade.isLoading = true;
                    pickupLocations.search({
                        storeId: blade.storeId,
                        keyword: $scope.filter.keyword,
                        sort: uiGridHelper.getSortExpression($scope),
                        skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                        take: $scope.pageSettings.itemsPerPageCount,
                    }, function (data) {
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

                // simple and advanced filtering
                var filter = $scope.filter = {};

                filter.criteriaChanged = function () {
                    if ($scope.pageSettings.currentPage > 1) {
                        $scope.pageSettings.currentPage = 1;
                    } else {
                        blade.refresh();
                    }
                };

                function showDetailBlade(node) {
                    var newBlade = {
                        id: 'pickupLocationDetail',
                        storeId: blade.storeId,
                        controller: 'virtoCommerce.shippingModule.pickupLocationDetailController',
                        template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/pickupLocation-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                }

                // ui-grid
                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                        uiGridHelper.bindRefreshOnSortChanged($scope);
                    });
                    bladeUtils.initializePagination($scope);
                };
            }]);
