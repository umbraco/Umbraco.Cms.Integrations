function urlInspectionToolController(editorState, notificationsService, umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource) {
    var vm = this;

    vm.loading = false;
    vm.showResults = false;
    vm.oauthConfiguration = {};
    vm.inspectionResult = {};

    // build default url inspection object
    vm.inspectionObj = {
        urls: editorState.current.urls,
        inspectionUrl: editorState.current.urls[0].text,
        siteUrl: window.location.origin,
        languageCode: editorState.current.urls[0].culture,
        multipleUrls: editorState.current.urls.length > 1,
        enabled: false
    };

    // get oauth configuration
    umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getOAuthConfiguration().then(function(response) {
        vm.oauthConfiguration = response;
    });

    vm.onConnectClick = function() {
        vm.authorizationWindow = window.open(vm.oauthConfiguration.authorizationUrl,
            "GoogleSearchConsole_Authorize",
            "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    vm.onRevokeToken = function() {
        revokeToken();
    }

    vm.onInspect = function () {

        vm.loading = true;

        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.inspect(vm.inspectionObj.inspectionUrl, vm.inspectionObj.siteUrl, vm.inspectionObj.languageCode)
            .then(function (response) {

                vm.loading = false;

                if (response.error !== undefined && response.error !== null) {

                    notificationsService.warning(response.error.status, response.error.message);

                    // if token expired -> refresh access token
                    if (response.error.code === "401") {
                        vm.isConnected = false;

                        // refresh access token
                        refreshAccessToken();
                    }
                } else {
                    vm.showResults = true;
                    vm.inspectionResult = response.inspectionResult;
                }
            });
    }

    vm.onEdit = function() {
        vm.inspectionObj.multipleUrls = false;
        vm.inspectionObj.enabled = true;
    }

    vm.onChangeInspectionUrl = function() {
        vm.inspectionObj.languageCode =
            editorState.current.urls.find(p => p.text === vm.inspectionObj.inspectionUrl).culture;
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "google:oauth:success") {

            var codeParam = "?code=";
            var scopeParam = "&scope=";

            vm.authorizationWindow.close();

            var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length, event.data.url.indexOf(scopeParam));

            umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getAccessToken(code).then(function (response) {
                if (response !== "error") {
                    vm.oauthConfiguration.isConnected = true;
                    notificationsService.success("Google Search Console Authorization", "Access Approved");
                } else {
                    notificationsService.error("Google Search Console Authorization", "Access Denied");
                }
            });
        } else if (event.data.type === "google:oauth:denied") {
            notificationsService.error("Google Search Console Authorization", "Access Denied");
            vm.oauthConfiguration.isConnected = false;
            vm.authorizationWindow.close();
        }

    }, false);

    function refreshAccessToken() {
        notificationsService.warning("Google Search Console Authorization", "Refreshing access token.");

        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.refreshAccessToken().then(
            function (response) {
                if (response.length !== "error") {

                    notificationsService.success("Google Search Console Authorization",
                        "Refresh access token - completed.");

                    vm.isConnected = true;
                } else
                    notificationsService.error("Google Search Console Authorization",
                        "An error has occurred.");
            });
    }

    function revokeToken() {
        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.revokeToken().then(function () {
            vm.oauthConfiguration.isConnected = false;
            vm.showResults = false;
        });
    }
}

angular.module("umbraco")
    .controller("UmbracoCms.Integrations.GoogleSearchConsole.UrlInspectionToolController", urlInspectionToolController)