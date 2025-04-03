angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationListController',
        ['$scope', 'platformWebApp.bladeNavigationService',
            'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper',
            'virtoCommerce.shippingModule.pickupLocations', 'platformWebApp.dialogService',
            function ($scope, bladeNavigationService, bladeUtils,
                uiGridConstants, uiGridHelper, pickupLocations, dialogService) {
                $scope.uiGridConstants = uiGridConstants;

                var blade = $scope.blade;
                blade.isLoading = false;

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

                blade.refresh = function (refreshWidget) {
                    blade.isLoading = true;
                    pickupLocations.search({
                        storeId: blade.storeId,
                        keyword: $scope.filter.keyword,
                        sort: uiGridHelper.getSortExpression($scope),
                        skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                        take: $scope.pageSettings.itemsPerPageCount,
                    }, function (data) {
                        blade.isLoading = false;
                        blade.currentEntities = data.results;
                        $scope.pageSettings.totalItems = data.totalCount;
                    }, function (error) {
                        bladeNavigationService.setError('Error ' + error.status, blade);
                    });
                    if (refreshWidget) {
                        blade.refreshWidget();
                    }
                }

                blade.selectNode = function (node) {
                    $scope.selectedNodeId = node.id;
                    var newBlade = {
                        id: 'pickupLocationDetail',
                        currentEntity: node,
                        storeId: blade.storeId,
                        controller: 'virtoCommerce.shippingModule.pickupLocationDetailController',
                        template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/pickupLocation-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, $scope.blade);
                };

                $scope.delete = function (item) {
                    var dialog = {
                        id: "confirmPickupLocationDelete",
                        title: "shipping.dialogs.pickup-location-delete.title",
                        message: "shipping.dialogs.pickup-location-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                blade.isLoading = true;
                                pickupLocations.remove({ id: item.id, storeId: blade.storeId }, function () {
                                    blade.refresh(true);
                                    blade.isLoading = false;
                                }, function (error) {
                                    bladeNavigationService.setError('Error ' + error.status, blade);
                                });
                            }
                        }
                    }
                    dialogService.showConfirmationDialog(dialog);
                }

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
                        currentEntity: node || {},
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
