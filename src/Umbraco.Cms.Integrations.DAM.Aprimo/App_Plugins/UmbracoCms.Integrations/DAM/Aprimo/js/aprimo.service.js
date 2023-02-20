function aprimoService() {
    return {
        browserIsSupported: function () {
            return !(window.navigator.userAgent.toLowerCase().indexOf("firefox") > -1
                || window.navigator.userAgent.toLowerCase().indexOf("trident") > -1);
        }
    }
}

angular.module("umbraco.services")
    .service("umbracoCmsIntegrationsDamAprimoService", aprimoService);