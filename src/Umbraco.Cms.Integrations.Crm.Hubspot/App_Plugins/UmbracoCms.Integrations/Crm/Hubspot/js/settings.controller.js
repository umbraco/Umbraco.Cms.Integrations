function settingsController($scope, notificationsService, umbracoCmsIntegrationsCrmHubspotService, umbracoCmsIntegrationsCrmHubspotResource) {

    var vm = this;

    const oauthName = "OAuth";

    vm.oauthSetup = {
        authEventHandled : 0
    };
    vm.status = {};

    $scope.$on('formSubmitting', function () {

        $scope.model.value.isValid = vm.status.isValid;
        $scope.model.value.type = vm.status.type;

    });

    umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration()
        .then(function(response) {

            vm.status = {
                isValid: response.isValid === true,
                type: response.type,
                description: umbracoCmsIntegrationsCrmHubspotService.configDescription[response.type.value],
                useOAuth: response.isValid === true && response.type.value === oauthName
            };

            if (vm.status.useOAuth) {
                validateOAuthSetup();
            }

            if (response.isValid !== true) {
                // if directive runs from property editor, the notifications should be hidden, because they will not be displayed properly behind the overlay window.
                // if directive runs from data type, the notifications are displayed
                if (typeof $scope.connected === "undefined")
                    notificationsService.warning("HubSpot Configuration",
                        "Invalid setup. Please review the API/OAuth settings.");
            }
        });

    vm.onConnectClick = function () {

        umbracoCmsIntegrationsCrmHubspotResource.getAuthorizationUrl().then(function (response) {
            vm.authWindow = window.open(response,
                "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");

        });
    }

    vm.onRevokeToken = function() {
        umbracoCmsIntegrationsCrmHubspotResource.revokeAccessToken().then(function (response) {
            vm.oauthSetup.isConnected = false;
            vm.oauthSetup.authEventHandled = 0;

            if(typeof $scope.connected === "undefined")
                notificationsService.success("HubSpot Configuration", "OAuth connection revoked.");

            if (typeof $scope.revoked === "function")
                $scope.revoked();
        });
    }

    // authorization listener
    window.addEventListener("message", function (event) {

        if (event.data.type === "hubspot:oauth:success") {

            if (vm.oauthSetup.authEventHandled != 0) return;

            umbracoCmsIntegrationsCrmHubspotResource.getAccessToken(event.data.code).then(function (response) {

                vm.oauthSetup.authEventHandled = 1;

                if (response.startsWith("Error:")) {
                    if (typeof $scope.connected === "undefined")
                        notificationsService.error("HubSpot Configuration", response);
                } else {
                    vm.oauthSetup.isConnected = true;
                    vm.status.description = umbracoCmsIntegrationsCrmHubspotService.configDescription.OAuthConnected;

                    if (typeof $scope.connected === "undefined")
                        notificationsService.success("HubSpot Configuration", "OAuth connected.");

                    if (typeof $scope.connected === "function")
                        $scope.connected();
                }
            });

        }
    }, false);


    function validateOAuthSetup() {
        umbracoCmsIntegrationsCrmHubspotResource.validateAccessToken().then(function (response) {

            vm.oauthSetup = {
                isConnected: response.isValid,
                isAccessTokenExpired: response.isExpired,
                isAccessTokenValid: response.isValid
            }

            if (vm.oauthSetup.isConnected === true && vm.oauthSetup.isAccessTokenValid === true) {
                vm.status.description = umbracoCmsIntegrationsCrmHubspotService.configDescription.OAuthConnected;
            }

            // refresh access token
            if (vm.oauthSetup.isAccessTokenExpired === true) {
                umbracoCmsIntegrationsCrmHubspotResource.refreshAccessToken().then(function (response) {
                });
            }

        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.SettingsController", settingsController);