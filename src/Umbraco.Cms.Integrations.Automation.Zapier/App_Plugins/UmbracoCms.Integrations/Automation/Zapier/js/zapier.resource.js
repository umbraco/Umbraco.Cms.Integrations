angular.module('umbraco.resources').factory('umbracoCmsIntegrationsAutomationZapierResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsAutomationZapier/ZapierConfig";

        return {
            getAllContentConfigs: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAll`), "Failed to get resource");
            }
        };
    }
);