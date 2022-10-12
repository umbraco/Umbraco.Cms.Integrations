angular.module("umbraco.directives")
    .directive("apiAccessConfiguration", function () {
        return {
            restrict: "E",
            templateUrl: "/App_Plugins/UmbracoCms.Integrations/Crm/ActiveCampaign/views/configuration.html"
        }
});