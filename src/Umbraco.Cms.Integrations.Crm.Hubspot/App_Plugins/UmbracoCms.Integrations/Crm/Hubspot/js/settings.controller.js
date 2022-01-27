function settingsController($scope, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {

    var vm = this;

    const apiKeyName = "API Key";
    const oAuthName = "OAuth";

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

        vm.useApi = selectedItem.name === apiKeyName;
        if (vm.useApi === true) {
            handleApiConfiguration();
        }

        vm.useOAuth = selectedItem.name === oAuthName;
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
                vm.oauthSetup.isConnected = true;
            });

        }
    }, false);


    // custom handlers
    function handleApiConfiguration() {
        umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration().then(function (response) {
            handleConfigurationValidation(response);
            if (vm.settingsValidation.isValid === false) {
                vm.useApi = false;
                vm.items.find(el => el.name === apiKeyName).checked = false;
            }
                
        });
    }

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

    function init() {
        vm.items = [{
            name: apiKeyName,
            description: "Use API key based setup.",
            checked: vm.useApi
        }, {
            name: oAuthName,
            description: "Use OAuth setup.",
            checked: vm.useOAuth
        }];
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