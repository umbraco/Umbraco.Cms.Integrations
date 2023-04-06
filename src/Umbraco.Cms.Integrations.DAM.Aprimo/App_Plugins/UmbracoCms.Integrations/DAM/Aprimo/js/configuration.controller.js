function configurationController($scope, notificationsService, umbracoCmsIntegrationsDamAprimoService, umbracoCmsIntegrationsDamAprimoResource) {
    var vm = this;

    vm.configuration = {};

    vm.useContentSelector = false;

    const useContentSelector = document.getElementById("useContentSelector");
    if ($scope.model.value != null && $scope.model.value.useContentSelector != null) {
        useContentSelector.checked = $scope.model.value.useContentSelector;
        vm.useContentSelector = $scope.model.value.useContentSelector;
    }

    const btnConnect = document.getElementById("btnConnect");
    const btnRevoke = document.getElementById("btnRevoke");

    checkApiConfiguration();

    $scope.$on('formSubmitting', function (ev) {
        $scope.model.value = {
            useContentSelector: vm.useContentSelector
        };
    });

    vm.onConnect = () => {
        window.addEventListener("message", handleAuthEvent, false);

        umbracoCmsIntegrationsDamAprimoResource.getAuthorizationUrl().then(function (response) {
            window.open(response,
                "Aprimo Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        });
    };

    vm.onRevoke = () => {
        window.removeEventListener("message", handleAuthEvent);
        umbracoCmsIntegrationsDamAprimoResource.revokeAccessToken().then(function () {
            vm.configuration.isAuthorized = false;
            vm.configuration.icon = "lock";
            vm.configuration.tag = "danger";
            vm.configuration.message = "Invalid access token.";

            toggleDisabledState(
                vm.configuration.isAuthorized
                    ? btnRevoke : btnConnect,
                vm.configuration.isAuthorized
                    ? btnConnect : btnRevoke);

            notificationsService.success("Aprimo", "Access token revoked.");
        });
    };

    vm.onUseContentSelector = () => {
        vm.useContentSelector = useContentSelector.checked;
    }

    function handleAuthEvent(event) {
        if (event.data.type === "aprimo:oauth:success") {
            notificationsService.success("Aprimo", "Connected.");
            checkApiConfiguration();
        } else if (event.data.type === "aprimo:oauth:error") {
            notificationsService.error("Aprimo", event.data.response);
        }
    }

    function checkApiConfiguration() {
        umbracoCmsIntegrationsDamAprimoResource.checkApiConfiguration().then(function (response) {

            vm.configuration = {
                isAuthorized: response.isAuthorized,
                icon: response.failure ? "lock" : "unlock",
                tag: response.failure ? "danger" : "positive",
                message: response.failure ? response.error : "Connected.",
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