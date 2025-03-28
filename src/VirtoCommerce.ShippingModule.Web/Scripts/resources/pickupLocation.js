angular.module('virtoCommerce.shippingModule')
    .factory('virtoCommerce.shippingModule.pickupLocations', ['$resource', function ($resource) {
        return $resource('api/shipping/pickup-locations', {}, {
            search: { method: 'POST', url: 'api/shipping/pickup-locations/search' },
            get: { method: 'GET', url: 'api/shipping/pickup-locations/:storeId/:id' },
            remove: { method: 'DELETE', url: 'api/shipping/pickup-locations/:storeId/:id' },
            update: { method: 'PUT' },
        });
    }]);
