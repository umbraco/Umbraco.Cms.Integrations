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
                query: function (entityTypeId, fieldTypeIds) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/Query`, {
                            entityTypeId: entityTypeId,
                            fieldTypeIds: fieldTypeIds
                        }), "Failed to access resource.")
                },
                fetchData: function (request) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/FetchData`, request), "Failed to access resource.")
                }
            };
        });