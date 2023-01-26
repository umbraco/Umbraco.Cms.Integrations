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
                query: function (configuration, culture) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/Query`, {
                            entityTypeId: configuration.entityType,
                            fieldTypes: configuration.fieldTypes,
                            linkedTypes: configuration.linkedTypes,
                            culture: culture
                        }), "Failed to access resource.")
                },
                fetchData: function (request) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(`${apiEndpoint}/FetchData`, request), "Failed to access resource.")
                },
                fetchEntityData: function (entityId) {
                    return umbRequestHelper.resourcePromise(
                        $http.get(`${apiEndpoint}/FetchEntityData?entityId=${entityId}`), "Failed to access resource.")
                }
            };
        });