function zapierController(umbracoCmsIntegrationsAutomationZapierResource) {

    var vm = this;

    vm.loading = false;
    vm.formsExtensionInstalled = false;
    vm.contentConfigs = [];
    vm.formConfigs = [];

    getContentConfigs();

    umbracoCmsIntegrationsAutomationZapierResource.checkFormsExtension().then(function (response) {
        vm.formsExtensionInstalled = response;

        if (response) {
            getFormConfigs();
        }
    });

    function getContentConfigs() {
        vm.loading = true;
        umbracoCmsIntegrationsAutomationZapierResource.getAllContentConfigs().then(function (response) {
            vm.contentConfigs = response;
            vm.loading = false;
        });
    }

    function getFormConfigs() {
        vm.loading = true;
        umbracoCmsIntegrationsAutomationZapierResource.getAllFormConfigs().then(function (response) {
            vm.formConfigs = response;
            vm.loading = false;
        });
    }

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Automation.Zapier.ZapierConfigController", zapierController);