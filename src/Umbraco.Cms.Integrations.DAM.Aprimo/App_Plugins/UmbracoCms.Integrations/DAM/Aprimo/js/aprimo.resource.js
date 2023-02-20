angular.module("umbraco.resources").factory("umbracoCmsIntegrationsDamAprimoResource",
    function ($http, umbRequestHelper) {
        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsDamAprimo/Assets";

        return {
            checkApiConfiguration: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/CheckApiConfiguration`),
                    "Failed to access resource");
            },
            getAuthorizationUrl: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                    "Failed to access resource");
            },
            getContentSelectorUrl: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetContentSelectorUrl`),
                    "Failed to access resource");
            },
            getAccessToken: function (authorizationCode) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }),
                    "Failed to access resource");
            },
            refreshAccessToken: function () {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/RefreshAccessToken`),
                    "Failed");
            },
            getRecords: function (page) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetRecords?page=${page}`),
                    "Failed to access resource");
            },
            getRecordDetails: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetRecordDetails?id=${id}`),
                    "Failed to access resource");
            }
        };
    });