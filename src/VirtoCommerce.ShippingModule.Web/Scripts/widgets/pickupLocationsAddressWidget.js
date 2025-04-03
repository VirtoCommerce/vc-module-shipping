angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationsAddressWidgetController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService',
            function ($scope, bladeNavigationService, metaFormsService) {
                var blade = $scope.widget.blade;

                $scope.address = blade.currentEntity.address;
                var addressMetaFields = [
                    {
                        name: 'countryCode',
                        templateUrl: 'countrySelector.html',
                        priority: 1
                    }, {
                        templateUrl: 'countryRegionSelector.html',
                        priority: 2
                    }, {
                        name: 'city',
                        title: 'shipping.widgets.pickupLocationAddress.address-detail.city',
                        valueType: 'ShortText',
                        isRequired: true,
                        priority: 3
                    }, {
                        name: 'line1',
                        title: 'shipping.widgets.pickupLocationAddress.address-detail.address1',
                        valueType: 'ShortText',
                        priority: 4
                    }, {
                        name: 'line2',
                        title: 'shipping.widgets.pickupLocationAddress.address-detail.address2',
                        valueType: 'ShortText',
                        priority: 5
                    }, {
                        name: 'postalCode',
                        title: 'shipping.widgets.pickupLocationAddress.address-detail.zip-code',
                        valueType: 'ShortText',
                        priority: 6
                    }];

                var metaFields = metaFormsService.getMetaFields('pickupLocationAddressWidget');
                if (metaFields && metaFields.length) {
                    addressMetaFields = _.sortBy(addressMetaFields.concat(metaFields), 'priority');
                }

                $scope.openBlade = function () {
                    var newBlade = {
                        id: 'coreAddressDetail',
                        currentEntity: $scope.address ? $scope.address : { isNew: true },
                        metaFields: addressMetaFields,
                        title: blade.title,
                        controller: 'virtoCommerce.coreModule.common.coreAddressDetailController',
                        confirmChangesFn: function (address) {

                            blade.currentEntity.address = address;
                            address.isEmpty = false;
                            if (blade.confirmChangesFn) {
                                blade.confirmChangesFn(address);
                            }
                        },
                        deleteFn: function (address) {
                            blade.currentEntity.address = {};
                        },
                        template: 'Modules/$(VirtoCommerce.Core)/Scripts/common/blades/address-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, $scope.blade);
                };

                $scope.$watch('blade.currentEntity', function (data) {
                    if (data && data.address) {
                        $scope.address = data.address;
                    }
                });


            }]);
