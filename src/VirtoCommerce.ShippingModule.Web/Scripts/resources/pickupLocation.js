angular.module('virtoCommerce.shippingModule')
    .factory('virtoCommerce.shippingModule.pickupLocations', ['$resource', function ($resource) {
        return $resource('api/shipping/pickup-locations/:storeId', {}, {
            getAll: { method: 'GET', isArray: true },
            search: { method: 'POST', url: 'api/shipping/pickup-locations/search' },
            get: { url: 'api/shipping/pickup-locations/:id' },
            update: { method: 'PUT' },
        });
    }]);
