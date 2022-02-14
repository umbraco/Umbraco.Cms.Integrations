angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmHubspotResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms";

        return {
            checkApiConfiguration: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/CheckConfiguration`),
                    "Failed to get resource");
            },
            getHubspotFormsList: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAll`), "");
            },
            getAuthorizationUrl: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                    "Failed");
            },
            getAccessToken: function(authorizationCode) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }),
                    "Failed");
            },
            refreshAccessToken: function () {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/RefreshAccessToken`),
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
            getHubspotFormsListOAuth: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAllOAuth`),
                    "Failed");
            }
        };
    }
);