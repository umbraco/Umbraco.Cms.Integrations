function configurationController($scope, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {
    var vm = this;

    vm.oauthConfig = {};

    umbracoCmsIntegrationsCrmDynamicsResource.checkOAuthConfiguration().then(function (response) {
        if (response && response.isAuthorized) {
            vm.oauthConfig.isConnected = true;
            vm.oauthConfig.fullName = response.fullName;

            if (typeof $scope.connected === "function")
                $scope.connected();
        }
    });

    vm.onConnectClick = function () {

        umbracoCmsIntegrationsCrmDynamicsResource.getAuthorizationUrl().then(function (response) {
            vm.authWindow = window.open(response,
                "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        });
    }

    vm.onRevokeToken = function () {

        umbracoCmsIntegrationsCrmDynamicsResource.revokeAccessToken().then(function () {
            vm.oauthConfig.isConnected = false;
            notificationsService.success("Dynamics Configuration", "OAuth connection revoked.");

            if (typeof $scope.revoked === "function")
                $scope.revoked();
        });
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "hubspot:oauth:success") {

            umbracoCmsIntegrationsCrmDynamicsResource.getAccessToken(event.data.code).then(function (response) {
                if (response.startsWith("Error:")) {
                    notificationsService.error("Dynamics Configuration", response);
                } else {
                    vm.oauthConfig.isConnected = true;
                    
                    notificationsService.success("Dynamics Configuration", "OAuth connected.");

                    umbracoCmsIntegrationsCrmDynamicsResource.getSystemUserFullName().then(function(response) {
                        vm.oauthConfig.fullName = response;
                    });

                    if (typeof $scope.connected === "function")
                        $scope.connected();
                }
            });
        }
    }, false);
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Dynamics.ConfigurationController", configurationController);