function settingsController($scope, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {

    var vm = this;

    const oauthName = "OAuth";
    const configDescription = {
        API: "An API key is configured and will be used to connect to your HubSpot account.",
        OAuth:
            "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
        None: "No API or OAuth configuration could be found. Please review your settings before continuing.",
        OAuthConnected:
            "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'"
    };

    vm.oauthSetup = {};
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
                description: configDescription[response.type.value],
                useOAuth: response.isValid === true && response.type.value === oauthName
            };

            console.log("STATUS: ", vm.status);

            if (vm.status.useOAuth) {
                validateOAuthSetup();
            }

            if (response.isValid === true) {
                notificationsService.success("Configuration", `${response.type.value} setup is configured.`);
            } else {
                notificationsService.warning("Configuration",
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
        umbracoCmsIntegrationsCrmHubspotResource.revokeToken().then(function (response) {
            vm.oauthSetup.isConnected = false;
        });
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "hubspot:oauth:success") {

            umbracoCmsIntegrationsCrmHubspotResource.getAccessToken(event.data.code).then(function (response) {
                vm.oauthSetup.isConnected = true;
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

            // refresh access token
            if (vm.oauthSetup.isExpired === true) {
                umbracoCmsIntegrationsCrmHubspotResource.refreshAccessToken().then(function (response) {
                });
            }

        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.SettingsController", settingsController);