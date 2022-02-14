angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsCommerceShopifyResource",
        function($http, umbRequestHelper) {
            const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCommerceShopify/Products";

            return {
                getProductsList: function() {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetList`), "Failed to get resource");
                }
            };
        });