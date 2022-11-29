angular.module('umbraco.resources').factory('umbracoCmsIntegrationsSearchAlgoliaResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsSearchAlgolia/Search";

        return {
            getIndices: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetIndices`),
                    "Failed");
            },
            saveIndex: function (id, name, contentData) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/SaveIndex`, { id: id, name: name, contentData: contentData }),
                    "Failed");
            },
            buildIndex: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/BuildIndex`, { id: id }),
                    "Failed");
            },
            deleteIndex: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.delete(`${apiEndpoint}/DeleteIndex?id=${id}`),
                    "Failed");
            },
            search: function (indexId, query) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/Search?indexId=${indexId}&query=${query}`),
                    "Failed");
            }
        };
    }
);