function semrushResources($http, umbRequestHelper) {

    return {
        ping: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "Test")), "Fail");
        },
        getTokenDetails: function() {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetTokenDetails")), "Fail");
        },
        getAccessToken: function (authorizationCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetAccessToken"), { code: authorizationCode }), "Fail");
        },
        getRelatedPhrases: function (selectedPhrase) {
            var url = umbRequestHelper.getApiUrl("umbracoCmsIntegrationsSemrushBaseUrl", "GetRelatedPhrases");
            return umbRequestHelper.resourcePromise(
                $http.get(url + "?phrase=" + selectedPhrase), "Fail");
        }
    };

}

angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsSemrushResources", semrushResources);