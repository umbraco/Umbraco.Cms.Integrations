angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmHubspotResource',
    function ($q, $http, umbRequestHelper) {
        return {
            getHubspotFormsList: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/UmbracoCmsIntegrationsCrmHubspot/Forms/GetAll"), "");
            }
        };
    }
); 