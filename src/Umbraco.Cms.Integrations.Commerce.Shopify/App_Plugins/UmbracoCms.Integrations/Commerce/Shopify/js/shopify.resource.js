angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsCommerceShopifyResource",
        function($http, umbRequestHelper) {
            const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCommerceShopify/Products";

            return {
                checkConfiguration: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/CheckConfiguration`),
                        "Failed to get resource");
                },
                getAuthorizationUrl: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                        "Failed");
                },
                getAccessToken: function (authorizationCode) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }),
                        "Failed");
                },
                getProductsList: function() {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetList`), "Failed to get resource");
                }
            };
        });