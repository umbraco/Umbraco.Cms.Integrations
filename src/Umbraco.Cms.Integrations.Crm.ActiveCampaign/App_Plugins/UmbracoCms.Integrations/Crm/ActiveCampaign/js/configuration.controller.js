function configurationController($scope, notificationsService, umbracoCmsIntegrationsCrmActiveCampaignResource) {
    var vm = this;

    umbracoCmsIntegrationsCrmActiveCampaignResource.checkApiAccess().then(function (response) {

        vm.status = response.isApiConfigurationValid 
            ? "Connected. Account name: " + response.account
            : "Invalid API configuration."
    });
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.ActiveCampaign.ConfigurationController", configurationController);