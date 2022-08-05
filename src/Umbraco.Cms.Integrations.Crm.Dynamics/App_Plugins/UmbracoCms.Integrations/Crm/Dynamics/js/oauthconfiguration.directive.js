angular.module("umbraco.directives")
    .directive("oauthConfiguration", function () {
        return {
            restrict: "E",
            scope: {
                "connected": "&onConnected",
                "revoked": "&onRevoked"
            },
            templateUrl: "/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/configuration.html"
        }
});