function configurationController(umbracoCmsIntegrationsDamAprimoResource) {
    var vm = this;


    vm.onConnect = () => {
        window.addEventListener("message", getAccessToken, false);

        umbracoCmsIntegrationsDamAprimoResource.getAuthorizationUrl().then(function (response) {
            window.open(response,
                "Aprimo Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        });
    };

    vm.onRevoke = () => {
        window.removeEventListener("message", getAccessToken);
    };

    function getAccessToken(event) {
        if (event.data.type === "hubspot:oauth:success") {

            umbracoCmsIntegrationsDamAprimoResource.getAccessToken(event.data.code).then(function (response) {
                
            });

        }
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.ConfigurationController", configurationController)