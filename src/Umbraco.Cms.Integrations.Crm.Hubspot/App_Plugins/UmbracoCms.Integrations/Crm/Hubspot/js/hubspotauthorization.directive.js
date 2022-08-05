angular.module("umbraco.directives")
    .directive("hubspotAuthorization", function () {
        return {
            restrict: "E",
            scope: {
                "connected": "&onConnected",
                "revoked": "&onRevoked"
            },
            templateUrl: "/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/settings.html"
        }
    });