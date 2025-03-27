angular.module('virtoCommerce.shippingModule')
    .factory('virtoCommerce.shippingModule.pickupLocations', ['$resource', function ($resource) {
        return $resource('api/shipping/pickup-locations/:storeId', {}, {
            search: { method: 'POST', url: 'api/shipping/pickup-locations/search' },
            update: { method: 'PUT' },
        });
    }]);
