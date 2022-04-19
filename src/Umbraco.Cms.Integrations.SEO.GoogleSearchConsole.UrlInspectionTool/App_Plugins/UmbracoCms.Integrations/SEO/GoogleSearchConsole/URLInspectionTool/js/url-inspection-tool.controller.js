function urlInspectionToolController(editorState, notificationsService, umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource) {
    var vm = this;

    vm.loading = false;
    vm.showResults = false;
    vm.oauthConfiguration = {};
    vm.inspectionResult = {};

    // get available cultures for current node
    vm.currentCultures = editorState.current.urls.map(p => p.culture);

    // build default url inspection object
    vm.siteUrl = location.href.slice(0, location.href.indexOf("umbraco") - 1);
    vm.urlInspection = {
        inspectionUrl: `${vm.siteUrl}${editorState.current.urls.find(p => p.culture === vm.currentCultures[0]).text}`,
        siteUrl: vm.siteUrl,
        languageCode: vm.currentCultures[0],
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

        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.inspect(vm.urlInspection.inspectionUrl, vm.urlInspection.siteUrl, vm.urlInspection.languageCode)
            .then(function (response) {

                vm.loading = false;

                if (response.error !== undefined && response.error !== null) {

                    notificationsService.warning(response.error.status, response.error.message);

                    // if token expired -> refresh access token
                    if (response.error.code === "401") {
                        vm.isConnected = false;

                        // refresh access token
                        notificationsService.warning("Google Search Console Authorization", "Refreshing access token.");

                        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.refreshAccessToken().then(
                            function(response) {
                                if (response.length !== "error") {
                                    vm.isConnected = true;
                                }
                            });
                    }
                } else {
                    vm.showResults = true;
                    vm.inspectionResult = response.inspectionResult;
                }
            });
    }

    vm.onEdit = function() {
        vm.urlInspection.enabled = true;
    }

    vm.onChangeLanguageCode = function() {
        vm.urlInspection.inspectionUrl =
            `${vm.siteUrl}${editorState.current.urls.find(p => p.culture === vm.urlInspection.languageCode).text}`;
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

    function revokeToken() {
        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.revokeToken().then(function () {
            vm.oauthConfiguration.isConnected = false;
            vm.showResults = false;
        });
    }
}

angular.module("umbraco")
    .controller("UmbracoCms.Integrations.GoogleSearchConsole.UrlInspectionToolController", urlInspectionToolController)