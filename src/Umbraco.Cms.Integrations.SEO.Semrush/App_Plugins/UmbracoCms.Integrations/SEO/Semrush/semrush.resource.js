function semrushResource($http, umbRequestHelper) {

    const apiEndpoint = "backoffice/UmbracoCmsIntegrationsSemrush/Semrush";

    return {
        ping: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/Ping`), "Fail");
        },
        getAuthorizationUrl: function() {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetAuthorizationUrl`), "Fail");
        },
        validateToken: function() {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/ValidateToken`), "Fail");
        },
        getTokenDetails: function() {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetTokenDetails`), "Fail");
        },
        revokeToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/RevokeToken`), "Fail");
        },
        getAccessToken: function (authorizationCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }), "Fail");
        },
        refreshAccessToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/RefreshAccessToken`), "Fail");
        },
        getRelatedPhrases: function (selectedPhrase, pageNumber, dataSource, method) {
            var url = `${apiEndpoint}/GetRelatedPhrases`;
            return umbRequestHelper.resourcePromise(
                $http.get(url + "?phrase=" + selectedPhrase + "&pageNumber=" + pageNumber + "&dataSource=" + dataSource + "&method=" + method), "Fail");
        },
        importDataSources: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/ImportDataSources`), "Fail");
        },
        getDataSources: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetDataSources`), "Fail");
        },
        getColumns: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetColumns`), "Fail");
        }
    };
}

angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsSemrushResource", semrushResource);