function semrushResource($http, umbRequestHelper) {

    return {
        ping: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "Test")), "Fail");
        },
        getAuthorizationUrl: function() {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetAuthorizationUrl")), "Fail");
        },
        validateToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "ValidateToken")), "Fail");
        },
        getTokenDetails: function() {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetTokenDetails")), "Fail");
        },
        revokeToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "RevokeToken")), "Fail");
        },
        getAccessToken: function (authorizationCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetAccessToken"), { code: authorizationCode }), "Fail");
        },
        refreshAccessToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "RefreshAccessToken")), "Fail");
        },
        getRelatedPhrases: function (selectedPhrase, pageNumber, dataSource, method) {
            var url = umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetRelatedPhrases");
            return umbRequestHelper.resourcePromise(
                $http.get(url + "?phrase=" + selectedPhrase + "&pageNumber=" + pageNumber + "&dataSource=" + dataSource + "&method=" + method), "Fail");
        },
        importDataSources: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "ImportDataSources")), "Fail");
        },
        getDataSources: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetDataSources")), "Fail");
        },
        getColumns: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetColumns")), "Fail");
        }
    };

}

angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsSemrushResource", semrushResource);