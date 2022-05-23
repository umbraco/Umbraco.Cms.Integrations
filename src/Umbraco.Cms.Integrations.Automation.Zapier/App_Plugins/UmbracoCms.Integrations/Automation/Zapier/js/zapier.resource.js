angular.module('umbraco.resources').factory('umbracoCmsIntegrationsAutomationZapierResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsAutomationZapier/ZapierConfig";

        return {
            getAllContentConfigs: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAll`),
                    "Failed to get resource");
            },
            checkFormsExtension: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/IsFormsExtensionInstalled`), "Failed to get resource");
            },
            getAllFormConfigs: function() {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAllForms`), "Failed to get resource");
            }
        };
    }
);