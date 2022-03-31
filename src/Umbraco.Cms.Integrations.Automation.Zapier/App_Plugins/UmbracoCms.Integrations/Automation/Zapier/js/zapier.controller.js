function zapierController($scope, notificationsService, umbracoCmsIntegrationsAutomationZapierResource, umbracoCmsIntegrationsAutomationZapierValidationService) {

    var vm = this;

    vm.loading = false;
    vm.contentTypes = [];
    vm.contentConfigs = [];

    getContentTypes();

    getContentConfigs();

    vm.onAdd = function () {
        const validationResult =
            umbracoCmsIntegrationsAutomationZapierValidationService.validateConfiguration(vm.webHookUrl,
                vm.selectedContentType);

        if (validationResult.length > 0) {
            notificationsService.warning("Zapier Content Config", validationResult);
            return;
        }
        
        umbracoCmsIntegrationsAutomationZapierResource.addConfig(vm.webHookUrl, vm.selectedContentType).then(function (response) {

            if (response.length > 0) {
                notificationsService.warning("Zapier Content Config", response);
                return;
            }

            getContentTypes();

            getContentConfigs();

            reset();
        });
    }

    vm.onTrigger = function (contentTypeName, webHookUrl) {

        vm.loading = true;

        umbracoCmsIntegrationsAutomationZapierResource.triggerWebHook(webHookUrl, contentTypeName).then(function(response) {

            vm.loading = false;

            if (response.length > 0)
                notificationsService.warning("WebHook Trigger", response);
            else
                notificationsService.success("WebHook Trigger", "WebHook triggered successfully. Please check your Zap trigger for the newly submitted request.");
        });
    }

    vm.onDelete = function(id) {
        umbracoCmsIntegrationsAutomationZapierResource.deleteConfig(id).then(function () {
            getContentTypes();

            getContentConfigs();
        });
    }

    function getContentTypes() {
        umbracoCmsIntegrationsAutomationZapierResource.getContentTypes().then(function (response) {
            vm.contentTypes = response;
        });
    }

    function getContentConfigs() {
        umbracoCmsIntegrationsAutomationZapierResource.getAllConfigs().then(function (response) {
            vm.contentConfigs = response;
        });
    }

    function reset() {
        vm.webHookUrl = "";
        vm.selectedContentType = "";
    }

    
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Automation.Zapier.ZapConfigController", zapierController);