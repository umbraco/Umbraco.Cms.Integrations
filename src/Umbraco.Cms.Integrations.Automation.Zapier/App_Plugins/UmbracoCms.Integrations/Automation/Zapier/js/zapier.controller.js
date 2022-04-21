function zapierController(umbracoCmsIntegrationsAutomationZapierResource) {

    var vm = this;

    vm.loading = false;
    vm.contentConfigs = [];

    getContentConfigs();

    function getContentConfigs() {
        vm.loading = true;
        umbracoCmsIntegrationsAutomationZapierResource.getAllContentConfigs().then(function (response) {
            vm.contentConfigs = response;
            vm.loading = false;
        });
    }

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Automation.Zapier.ZapierConfigController", zapierController);