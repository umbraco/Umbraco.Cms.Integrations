angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmHubspotResource',
    function ($http, umbRequestHelper) {
        return {
            getHubspotFormsList: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAll"), "");
            },
            checkApiConfiguration: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/CheckApiConfiguration"),
                    "Failed to get resource");
            },
            checkOAuthConfiguration: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/CheckOAuthConfiguration"),
                    "Failed to get resource");
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