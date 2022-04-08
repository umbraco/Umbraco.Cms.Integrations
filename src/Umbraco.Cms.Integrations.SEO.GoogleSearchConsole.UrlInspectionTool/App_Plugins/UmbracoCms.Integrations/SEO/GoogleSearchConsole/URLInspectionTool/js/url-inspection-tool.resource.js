function urlInspectionToolResource($http, umbRequestHelper) {

    const apiEndpoint = "backoffice/UmbracoCmsIntegrationsGoogleSearchConsole/UrlInspectionApi";

    return {
        getAuthorizationUrl: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                "Failed to retrieve resource");
        },
        getAccessToken: function (authorizationCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }), "Failed to retrieve resource");
        },
        revokeToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/RevokeToken`), "Failed to retrieve resource");
        },
    };
}

angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource", urlInspectionToolResource);