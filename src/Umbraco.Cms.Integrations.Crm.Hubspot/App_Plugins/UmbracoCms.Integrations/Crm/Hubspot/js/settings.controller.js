function settingsController($scope, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {

    var vm = this;

    const oauthName = "OAuth";

    vm.status = {};

    umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration().then(function (response) {

        vm.status = {
            isValid: response.isValid === true,
            type: response.type,
            description: response.isValid === true ? `${response.type.value} is configured.` : "Invalid configuration",
            useOAuth: response.isValid === true && response.type.value === oauthName
        };

        if (response.isValid === true) {
            notificationsService.success("Configuration", `${response.type.value} setup is configured.`);
        } else {
            notificationsService.warning("Configuration",
                "Invalid setup. Please review the API/OAuth settings.");
        }
    });

    //if (vm.useOAuth) {
    //    // validate token
    //    validateOAuthAccessToken();
    //}

    //$scope.$on('formSubmitting', function () {

    //    $scope.model.value.isValid = vm.status.isValid;
    //    $scope.model.value.type = vm.status.type;

    //});


    vm.onConnectClick = function () {

        umbracoCmsIntegrationsCrmHubspotResource.getAuthorizationUrl().then(function (response) {
            vm.authWindow = window.open(response,
                "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");

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


    // custom handlers

    function handleOAuthConfiguration() {
        umbracoCmsIntegrationsCrmHubspotResource.checkOAuthConfiguration().then(function (response) {
            handleConfigurationValidation(response);
            if (vm.settingsValidation.isValid === false) {
                vm.useOAuth = false;
                vm.items.find(el => el.name === oAuthName).checked = false;
            }
            
        });
    }

    function handleConfigurationValidation(result) {
        if (result === false) {
            vm.settingsValidation.isValid = false;
            notificationsService.warning("Settings", vm.settingsValidation.message);
        } else
            vm.settingsValidation.isValid = true;
    }

    function validateOAuthAccessToken() {
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