function settingsController($scope, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {

    var vm = this;

    vm.settingsValidation = {
        isValid: true,
        message: "Invalid configuration. Please review your website's configurations."
    };

    vm.useApi = $scope.model.value.useApi;
    vm.useOAuth = $scope.model.value.useOAuth;
    vm.oauthSetup = {};

    if (vm.useOAuth) {
        // validate token
        validateOAuthAccessToken();
    }

    init();

    $scope.$on('formSubmitting', function () {

        $scope.model.value.useApi = vm.useApi;
        $scope.model.value.useOAuth = vm.useOAuth;

    });

    vm.toggleAuthType = function (item) {

        let selectedItem = vm.items.find(el => el.name === item.name);

        var deselectedItem = vm.items.find(el => el.name !== item.name);
        deselectedItem.disabled = false;
        deselectedItem.checked = false;

        vm.useApi = selectedItem.name === "API Key";
        if (vm.useApi === true) {
            handleApiConfiguration();
        }

        vm.useOAuth = selectedItem.name === "OAuth";
        if (vm.useOAuth === true) {
            handleOAuthConfiguration();

            validateOAuthAccessToken();
        }
    }

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
                console.log(response);

                vm.isOAuthConnected = true;
            });

        }
    }, false);


    // custom handlers
    function handleApiConfiguration() {
        umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration().then(function (response) {
            handleConfigurationValidation(response);
        });
    }

    function handleOAuthConfiguration() {
        umbracoCmsIntegrationsCrmHubspotResource.checkOAuthConfiguration().then(function (response) {
            handleConfigurationValidation(response);
        });
    }

    function handleConfigurationValidation(result) {
        if (result === false) {
            vm.settingsValidation.isValid = false;
            notificationsService.warning("Settings", vm.settingsValidation.message);
        } else
            vm.settingsValidation.isValid = true;
    }

    function init() {
        vm.items = [{
            name: "API Key",
            description: "Use API key based setup.",
            checked: vm.useApi
        }, {
            name: "OAuth",
            description: "Use OAuth setup.",
            checked: vm.useOAuth
        }];
    }

    function validateOAuthAccessToken() {
        umbracoCmsIntegrationsCrmHubspotResource.validateAccessToken().then(function (response) {

            vm.oauthSetup = {
                isConnected: response.isAccessTokenValid,
                isAccessTokenExpired: response.isAccessTokenExpired,
                isAccessTokenValid: response.isAccessTokenValid
            }

            // refresh access token
            if (vm.oauthSetup.isAccessTokenExpired === true) {
                umbracoCmsIntegrationsCrmHubspotResource.refreshAccessToken().then(function (response) {
                    console.log(response);
                });
            }

        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.SettingsController", settingsController);