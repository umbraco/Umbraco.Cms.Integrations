angular.module('umbraco.resources').factory('umbracoCmsIntegrationsAutomationZapierResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsAutomationZapier/ZapConfig";

        return {
            getContentTypes: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetContentTypes`),
                    "Failed to get resource");
            },
            addConfig: function (webHookUrl, contentTypeName) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/Add`, { contentTypeName: contentTypeName, webHookUrl: webHookUrl }), "Failed to get resource");
            },
            getAllConfigs: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAll`), "Failed to get resource");
            },
            triggerWebHook: function(webHookUrl, contentTypeName) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/TriggerAsync`, { contentTypeName: contentTypeName, webHookUrl: webHookUrl }), "Failed to get resource");
            },
            deleteConfig: function(id) {
                return umbRequestHelper.resourcePromise(
                    $http.delete(`${apiEndpoint}/Delete?id=${id}`), "Failed to get resource");
            }
        };
    }
);