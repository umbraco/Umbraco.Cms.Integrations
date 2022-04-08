function urlInspectionToolController(notificationsService, umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource) {
    var vm = this;

    vm.loading = false;
    vm.isConnected = false;

    umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getAuthorizationUrl().then(function(response) {
        vm.authorizationUrl = response;
    });

    vm.onConnectClick = function() {
        vm.authorizationWindow = window.open(vm.authorizationUrl,
            "GoogleSearchConsole_Authorize",
            "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    vm.onRevokeToken = function() {
        revokeToken();
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "google:oauth:success") {

            var codeParam = "?code=";

            vm.authorizationWindow.close();

            var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length);

            umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getAccessToken(code).then(function (response) {
                if (response !== "error") {
                    vm.isConnected = true;
                    notificationsService.success("Google Search Console Authentication", "Access Approved");
                } else {
                    notificationsService.error("Google Search Console Authentication", "Access Denied");
                }
            });
        } else if (event.data.type === "google:oauth:denied") {
            vm.showError("Google Search Console Authentication", "Access Denied");

            vm.authWindow.close();
        }

    }, false);

    function revokeToken() {
        umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.revokeToken().then(function () {
            vm.isConnected = false;
        });
    }
}

angular.module("umbraco")
    .controller("UmbracoCms.Integrations.GoogleSearchConsole.UrlInspectionToolController", urlInspectionToolController)