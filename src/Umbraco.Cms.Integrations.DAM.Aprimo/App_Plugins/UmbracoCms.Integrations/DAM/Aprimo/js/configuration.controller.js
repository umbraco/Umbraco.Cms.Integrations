function configurationController($scope, notificationsService, umbracoCmsIntegrationsDamAprimoService, umbracoCmsIntegrationsDamAprimoResource) {
    var vm = this;

    vm.configuration = {};

    const useContentSelector = document.getElementById("useContentSelector");
    if ($scope.model.value != null && $scope.model.value.useContentSelector != null) {
        useContentSelector.checked = $scope.model.value.useContentSelector;
    }

    const btnConnect = document.getElementById("btnConnect");
    const btnRevoke = document.getElementById("btnRevoke");

    checkApiConfiguration();

    $scope.$on('formSubmitting', function (ev) {
        $scope.model.value = {
            useContentSelector: useContentSelector.checked
        };
    });

    vm.onConnect = () => {
        window.addEventListener("message", getAccessToken, false);

        umbracoCmsIntegrationsDamAprimoResource.getAuthorizationUrl().then(function (response) {
            window.open(response,
                "Aprimo Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        });
    };

    vm.onRevoke = () => {
        window.removeEventListener("message", getAccessToken);
        umbracoCmsIntegrationsDamAprimoResource.revokeAccessToken().then(function () {
            vm.configuration.isAuthorized = false;
            vm.configuration.icon = response.failure ? "lock" : "unlock";
            vm.configuration.tag = response.failure ? "danger" : "positive";

            notificationsService.success("Aprimo", "Access token revoked.");
        });
    };

    function getAccessToken(event) {
        if (event.data.type === "hubspot:oauth:success") {
            umbracoCmsIntegrationsDamAprimoResource.getAccessToken(event.data.code).then(function (response) {
                if (response.indexOf("Error") > -1)
                    notificationsService.error("Aprimo", response);
                else
                    checkApiConfiguration();
            });
        }
    }

    function checkApiConfiguration() {
        umbracoCmsIntegrationsDamAprimoResource.checkApiConfiguration().then(function (response) {

            vm.configuration = {
                isAuthorized: response.isAuthorized,
                icon: response.failure ? "lock" : "unlock",
                tag: response.failure ? "danger" : "positive",
                message: response.failure ? response.error : "Connected",
                browserIsSupported: umbracoCmsIntegrationsDamAprimoService.browserIsSupported()
            };

            toggleDisabledState(
                vm.configuration.isAuthorized
                    ? btnRevoke : btnConnect,
                vm.configuration.isAuthorized
                    ? btnConnect : btnRevoke);
        });
    }

    function toggleDisabledState(activeCtrl, disabledCtrl) {
        activeCtrl.removeAttribute("disabled");
        disabledCtrl.setAttribute("disabled", "");
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.ConfigurationController", configurationController)