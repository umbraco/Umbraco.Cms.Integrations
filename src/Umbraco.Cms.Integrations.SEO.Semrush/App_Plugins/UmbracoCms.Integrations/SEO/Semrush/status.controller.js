(() => {

    function statusController($scope, $window, umbracoCmsIntegrationsSemrushResource) {

        var vm = this;

        umbracoCmsIntegrationsSemrushResource.getTokenDetails().then(function (response) {

            console.log(response);

            vm.isAccessTokenAvailable = response.isAccessTokenAvailable;
            vm.AccessToken = response.access_token;

        });

        vm.OnRevokeToken = function() {
            umbracoCmsIntegrationsSemrushResource.revokeToken().then(function() {
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