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
                revokeAccessToken: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/RevokeAccessToken`),
                        "Failed");
                },
                validateAccessToken: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/ValidateAccessToken`),
                        "Failed");
                },
                getProductsList: function(pageInfo) {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetList?pageInfo=${pageInfo}`), "Failed to get resource");
                },
                getProductsByIds: function (ids) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/GetListByIds`, { ids: ids }), "Failed to get resource");
                },
                getTotalPages: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetTotalPages`), "Failed to get resource");
                }
            };
        });