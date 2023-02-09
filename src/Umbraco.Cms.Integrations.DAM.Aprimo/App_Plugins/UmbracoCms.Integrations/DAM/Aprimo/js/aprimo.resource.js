angular.module("umbraco.resources").factory("umbracoCmsIntegrationsDamAprimoResource",
    function ($http, umbRequestHelper) {
        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsDamAprimo/Assets";

        return {
            getAuthorizationUrl: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                    "Failed to access resource");
            },
            getAccessToken: function (authorizationCode) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }),
                    "Failed");
            },
        };
    });