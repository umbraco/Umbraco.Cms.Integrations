function zapierController(umbracoCmsIntegrationsAutomationZapierResource) {

    var vm = this;

    vm.loading = false;
    vm.formsExtensionInstalled = false;

    vm.subscriptionHooks = [];

    // check if forms is installed
    umbracoCmsIntegrationsAutomationZapierResource.checkFormsExtension().then(function (response) {
        vm.formsExtensionInstalled = response;
    });

    // load subscription hooks
    vm.loading = true;
    umbracoCmsIntegrationsAutomationZapierResource.getAll().then(function (response) {

        vm.subscriptionHooks = response;

        vm.loading = false;
    });

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Automation.Zapier.ZapierConfigController", zapierController);