angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.pickupLocationDetailController',
        ['$scope', '$injector', '$timeout', '$translate', 'platformWebApp.bladeNavigationService',
            'virtoCommerce.shippingModule.pickupLocations', 'FileUploader',
            function ($scope, $injector, $timeout, $translate, bladeNavigationService, pickupLocations, FileUploader) {
                var blade = $scope.blade;
                $scope.inventoryModuleInstalled = $injector.has('virtoCommerce.inventoryModule.fulfillments')

                function initializeBlade(data) {
                    blade.currentEntity = angular.copy(data);
                    blade.currentEntityId = blade.currentEntity.id; // Required for index widget
                    blade.origEntity = data;
                    blade.isLoading = false;

                    if (!blade.title) {
                        blade.title = $translate.instant('shipping.blades.pickup-location-detail.labels.create');
                    }
                }

                blade.refresh = function (refreshParent, refreshWidget) {
                    if (!!blade.currentEntity.id) {
                        blade.isLoading = true;
                        pickupLocations.get({ id: blade.currentEntity.id, storeId: blade.storeId }, function (data) {
                            initializeBlade(data);
                            if (refreshParent) {
                                blade.parentBlade.refresh(refreshWidget);
                            }
                        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    } else {
                        initializeBlade({});
                    }
                }

                function canSave() {
                    return $scope.formScope && $scope.formScope.$valid && isDirty();
                }

                function isDirty() {
                    return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
                }

                function isAddressValid() {
                    var model = blade.currentEntity;
                    return model.address && model.address.countryName
                        && model.address.city && model.address.postalCode;
                }

                blade.onClose = function (closeCallback) {
                    bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade,
                        $scope.saveChanges, closeCallback,
                        "shipping.dialogs.pickup-location-save.title", "shipping.dialogs.pickup-location-save.message");
                };

                $scope.cancelChanges = function () {
                    $scope.bladeClose();
                };

                $scope.saveChanges = function () {
                    blade.isLoading = true;
                    blade.currentEntity.storeId = blade.storeId;

                    if (blade.currentEntity.isActive && !isAddressValid()) {
                        blade.currentEntity.isActive = false;
                    }

                    if (blade.currentEntity.id) {
                        pickupLocations.update({}, blade.currentEntity, function (data) {
                            blade.refresh(true, false);
                        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    } else {
                        pickupLocations.create({}, blade.currentEntity, function (data) {
                            blade.currentEntity.id = data.id;
                            blade.refresh(true, true);
                        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    }
                };

                $scope.setForm = function (form) {
                    $scope.formScope = form;
                }

                $scope.onIsActiveChange = function () {
                    // pickup point is not active if address is incorrect
                    if (!isAddressValid()) {
                        blade.error = $translate.instant('shipping.blades.pickup-location-detail.errors.checkAddress');
                        $timeout(function () {
                            blade.error = undefined;
                        }, 4000);
                        $timeout(function () {
                            blade.currentEntity.isActive = false;
                        }, 400);
                    }
                };

                var contentType = 'image';
                $scope.fileUploader = new FileUploader({
                    url: `api/assets?folderUrl=cms-content/${contentType}/assets`,
                    headers: { Accept: 'application/json' },
                    autoUpload: true,
                    removeAfterUpload: true,
                    onBeforeUploadItem: function (fileItem) {
                        blade.isLoading = true;
                    },
                    onSuccessItem: function (fileItem, response, status, headers) {
                        $scope.$broadcast('filesUploaded', { items: response });
                    },
                    onErrorItem: function (fileItem, response, status, headers) {
                        bladeNavigationService.setError(`${fileItem._file.name} failed: ${(response.message ? response.message : status)}`, blade);
                    },
                    onCompleteAll: function () {
                        blade.isLoading = false;
                    }
                });


                $scope.getDictionaryValues = function (setting, callback) {
                    callback(setting.allowedValues);
                };

                blade.headIcon = 'fa fa-archive';

                blade.toolbarCommands = [
                    {
                        name: "platform.commands.save",
                        icon: 'fa fa-save',
                        executeMethod: $scope.saveChanges,
                        canExecuteMethod: canSave,
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.reset",
                        icon: 'fa fa-undo',
                        executeMethod: function () {
                            angular.copy(blade.origEntity, blade.currentEntity);
                            $scope.$broadcast('resetContent',
                                {
                                    body: blade.currentEntity.workingHours,
                                    id: "workingHoursEditor"
                                });

                        },
                        canExecuteMethod: isDirty,
                        permission: blade.updatePermission
                    }
                ];

                blade.fetchFulfillmentCenters = function (criteria) {
                    criteria.storeId = blade.storeId;
                    var fulfillmentsApi = $injector.get('virtoCommerce.inventoryModule.fulfillments');

                    return fulfillmentsApi.search(criteria);
                }

                blade.refresh();

            }]);
