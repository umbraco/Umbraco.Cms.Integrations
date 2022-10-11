angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmActiveCampaignResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCrmActiveCampaign/Forms";

        return {
            checkApiAccess: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/CheckApiAccess`),
                    "Failed");
            },
            getForms: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetForms`),
                    "Failed");
            }
        };
    }
);