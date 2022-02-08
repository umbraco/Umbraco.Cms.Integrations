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
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAll"), "");
            },
            getAuthorizationUrl: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAuthorizationUrl"),
                    "Failed");
            },
            getAccessToken: function(authorizationCode) {
                return umbRequestHelper.resourcePromise(
                    $http.post("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAccessToken", { code: authorizationCode }),
                    "Failed");
            },
            refreshAccessToken: function () {
                return umbRequestHelper.resourcePromise(
                    $http.post("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/RefreshAccessToken"),
                    "Failed");
            },
            validateAccessToken: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/ValidateAccessToken"),
                    "Failed");
            },
            getHubspotFormsListOAuth: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAllOAuth"),
                    "Failed");
            }
        };
    }
);