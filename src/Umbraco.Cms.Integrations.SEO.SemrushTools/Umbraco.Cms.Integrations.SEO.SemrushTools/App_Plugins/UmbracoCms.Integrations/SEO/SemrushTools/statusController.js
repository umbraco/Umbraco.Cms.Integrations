(() => {

    function statusController($scope, $window, umbracoCmsIntegrationsSemrushResources) {

        var vm = this;

        umbracoCmsIntegrationsSemrushResources.getTokenDetails().then(function (response) {

            console.log(response);

            vm.IsAccessTokenAvailable = response.IsAccessTokenAvailable;
            vm.AccessToken = response.access_token;

        });

        vm.OnRevokeToken = function() {
            umbracoCmsIntegrationsSemrushResources.revokeToken().then(function() {
                vm.close();
            });
        }

        vm.close = function () {
            if ($scope.model.close) {
                $scope.model.close();
            }
        }
    }

    angular.module('umbraco')
        .controller('UmbracoCms.Integrations.SemrushStatusController', statusController);
})();