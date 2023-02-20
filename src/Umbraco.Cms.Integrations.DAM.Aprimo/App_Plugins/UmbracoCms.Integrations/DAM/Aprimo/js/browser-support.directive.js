angular.module("umbraco.directives")
    .directive("browserSupport", function () {
        return {
            restrict: "E",
            templateUrl: "/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/browser-support.html"
        }
    });