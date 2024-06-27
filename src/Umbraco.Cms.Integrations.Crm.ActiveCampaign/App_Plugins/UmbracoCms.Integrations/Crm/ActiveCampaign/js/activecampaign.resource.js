angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmActiveCampaignResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCrmActiveCampaign/Forms";

        return {
            checkApiAccess: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/CheckApiAccess`),
                    "Failed");
            },
            getForms: function (page) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetForms?page=${page}`),
                    "Failed");
            },
            getForm: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetForm?id=${id}`),
                    "Failed");
            }
        };
    }
);