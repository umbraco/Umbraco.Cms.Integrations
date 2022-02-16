function productPickerSettingsController($scope, notificationsService, umbracoCmsIntegrationsCommerceShopifyService, umbracoCmsIntegrationsCommerceShopifyResource) {

    var vm = this;

    const oauthName = "OAuth";

    vm.oauthSetup = {};
    vm.status = {};

    $scope.$on('formSubmitting', function () {

        $scope.model.value.isValid = vm.status.isValid;
        $scope.model.value.type = vm.status.type;

    });

    umbracoCmsIntegrationsCommerceShopifyResource.checkConfiguration()
        .then(function (response) {

            vm.status = {
                isValid: response.isValid === true,
                type: response.type,
                description: umbracoCmsIntegrationsCommerceShopifyService.configDescription[response.type.value],
                useOAuth: response.isValid === true && response.type.value === oauthName
            };

            console.log("STATUS: ", vm.status);

            //if (vm.status.useOAuth) {
            //    validateOAuthSetup();
            //}

            if (response.isValid !== true) {
                notificationsService.warning("Shopify Configuration",
                    "Invalid setup. Please review the API/OAuth settings.");
            }
        });

    vm.onConnectClick = function () {

        umbracoCmsIntegrationsCommerceShopifyResource.getAuthorizationUrl().then(function (response) {
            vm.authWindow = window.open(response,
                "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");

        });
    }

    vm.onRevokeToken = function () {
        //umbracoCmsIntegrationsCommerceShopifyResource.revokeAccessToken().then(function (response) {
        //    vm.oauthSetup.isConnected = false;
        //    notificationsService.success("HubSpot Configuration", "OAuth connection revoked.");
        //});
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "shopify:oauth:success") {
            umbracoCmsIntegrationsCommerceShopifyResource.getAccessToken(event.data.code).then(function (response) {
                if (response.startsWith("Error:")) {
                    notificationsService.error("Shopify Configuration", response);
                } else {
                    vm.oauthSetup.isConnected = true;
                    vm.status.description = umbracoCmsIntegrationsCommerceShopifyService.configDescription.OAuthConnected;
                    notificationsService.success("Shopify Configuration", "OAuth connected.");
                }
            });

        }
    }, false);


    //function validateOAuthSetup() {
    //    umbracoCmsIntegrationsCrmHubspotResource.validateAccessToken().then(function (response) {

    //        vm.oauthSetup = {
    //            isConnected: response.isValid,
    //            isAccessTokenExpired: response.isExpired,
    //            isAccessTokenValid: response.isValid
    //        }

    //        if (vm.oauthSetup.isConnected === true && vm.oauthSetup.isAccessTokenValid === true) {
    //            vm.status.description = umbracoCmsIntegrationsCrmHubspotService.configDescription.OAuthConnected;
    //        }

    //        // refresh access token
    //        if (vm.oauthSetup.isAccessTokenExpired === true) {
    //            umbracoCmsIntegrationsCrmHubspotResource.refreshAccessToken().then(function (response) {
    //            });
    //        }

    //    });
    //}
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Commerce.Shopify.ProductPickerSettingsController", productPickerSettingsController);