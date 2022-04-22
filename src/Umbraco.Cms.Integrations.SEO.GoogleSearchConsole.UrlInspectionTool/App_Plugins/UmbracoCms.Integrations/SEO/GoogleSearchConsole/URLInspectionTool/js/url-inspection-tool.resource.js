function urlInspectionToolResource($http, umbRequestHelper) {

    const apiEndpoint = "backoffice/UmbracoCmsIntegrationsGoogleSearchConsole/UrlInspectionApi";

    return {
        getOAuthConfiguration: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(`${apiEndpoint}/GetOAuthConfiguration`),
                "Failed to retrieve resource");
        },
        getAccessToken: function (authorizationCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }), "Failed to retrieve resource");
        },
        refreshAccessToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/RefreshAccessToken`), "Failed to retrieve resource");
        },
        revokeToken: function () {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/RevokeToken`), "Failed to retrieve resource");
        },
        inspect: function (inspectionUrl, siteUrl, languageCode) {
            return umbRequestHelper.resourcePromise(
                $http.post(`${apiEndpoint}/Inspect`, { inspectionUrl: inspectionUrl, siteUrl: siteUrl, languageCode: languageCode }), "Failed to retrieve resource");
        }
    };
}

angular.module("umbraco.resources")
    .factory("umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource", urlInspectionToolResource);