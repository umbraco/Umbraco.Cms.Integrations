angular.module('umbraco.resources')
    .factory('umbracoCmsIntegrationsPimInriverResource',
        function ($http, umbRequestHelper) {

            const apiEndpoint = "backoffice/UmbracoCmsIntegrationsPimInriver/Entity";

            return {
                checkApiAccess: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/CheckApiAccess`), "Failed to access resource");
                },
                getEntityTypes: function () {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetEntityTypes`), "Failed to access resource.")
                },
                getEntitySummary: function (id) {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/GetEntitySummary?id=${id}`), "Failed to access resource.")
                },
                query: function (entityTypeId) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/Query`, {
                            entityTypeId: entityTypeId
                        }), "Failed to access resource.")
                }
            };
        });