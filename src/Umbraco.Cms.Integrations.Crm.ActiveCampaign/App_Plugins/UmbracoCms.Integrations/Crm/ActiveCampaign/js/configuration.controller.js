function configurationController($scope, notificationsService, umbracoCmsIntegrationsCrmActiveCampaignResource) {
    var vm = this;

    umbracoCmsIntegrationsCrmActiveCampaignResource.checkApiAccess().then(function (response) {

        if (response.isApiConfigurationValid)
            vm.account = response.account;

        vm.status = response.isApiConfigurationValid 
            ? "Connected. Account name: "
            : "Invalid API configuration."
    });
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.ActiveCampaign.ConfigurationController", configurationController);