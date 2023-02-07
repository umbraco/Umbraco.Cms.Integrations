function urlInspectionToolController(editorState, notificationsService, umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource) {
    var vm = this;

    vm.loading = false;
    vm.showResults = false;
    vm.oauthConfiguration = {};
    vm.inspectionResult = {};
    vm.oauthSuccessEventCount = 0;

    // build default url inspection object
    var nodeUrls = getUrls();
    vm.inspectionObj = {
        urls: nodeUrls,
        inspectionUrl: nodeUrls[0],
        siteUrl: window.location.origin,
        languageCode: editorState.current.urls[0].culture,
        multipleUrls: editorState.current.urls.length > 1,
        enabled: false
    };

    // get oauth configuration
    umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getOAuthConfiguration().then(function (response) {
        vm.oauthConfiguration = response;
    });

    vm.onConnectClick = function () {
        window.addEventListener("message", getAccessToken, false);
        vm.authorizationWindow = window.open(vm.oauthConfiguration.authorizationUrl,
            "GoogleSearchConsole_Authorize",
            "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    vm.onRevokeToken = function () {
        revokeToken();

        vm.oauthSuccessEventCount = 0;
        window.removeEventListener("message", getAccessToken);
    }

    vm.onInspect = function () {

        vm.loading = true;

        // check if url is relative
        if (isRelativeUrl(vm.inspectionObj.inspectionUrl))
            vm.inspectionObj.inspectionUrl = `${vm.inspectionObj.siteUrl}${vm.inspectionObj.inspectionUrl}`;

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

    vm.onEdit = function () {
        vm.inspectionObj.multipleUrls = false;
        vm.inspectionObj.enabled = true;
    }

    vm.onChangeInspectionUrl = function () {
        vm.inspectionObj.languageCode =
            editorState.current.urls.find(p => p.text === vm.inspectionObj.inspectionUrl).culture;
    }

    function getAccessToken(event) {
        if (event.data.type === "google:oauth:success") {
            vm.oauthSuccessEventCount += 1;

            var codeParam = "?code=";
            var scopeParam = "&scope=";

            vm.authorizationWindow.close();

            var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length, event.data.url.indexOf(scopeParam));

            if (vm.oauthSuccessEventCount == 1) {
                umbracoCmsIntegrationsGoogleSearchConsoleUrlInspectionToolResource.getAccessToken(code).then(function (response) {
                    if (response !== "error") {
                        vm.oauthConfiguration.isConnected = true;
                        notificationsService.success("Google Search Console Authorization", "Access Approved");
                    } else {
                        notificationsService.error("Google Search Console Authorization", "Access Denied");
                    }
                });
            }
        } else if (event.data.type === "google:oauth:denied") {
            notificationsService.error("Google Search Console Authorization", "Access Denied");
            vm.oauthConfiguration.isConnected = false;
            vm.authorizationWindow.close();
        }
    }

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

    function isRelativeUrl(url) {
        var regExp = new RegExp('^(?:[a-z]+:)?//', 'i');
        return !regExp.test(url);
    }

    function getUrls() {
        var arr = [];

        for (var i = 0; i < editorState.current.urls.length; i++) {
            var url = isRelativeUrl(editorState.current.urls[i].text)
                ? `${window.location.origin}${editorState.current.urls[i].text}`
                : editorState.current.urls[i].text;

            if (arr.indexOf(url) === -1) {
                arr.push(url);
            }
        }

        return arr;
    }
}

angular.module("umbraco")
    .controller("UmbracoCms.Integrations.GoogleSearchConsole.UrlInspectionToolController", urlInspectionToolController)