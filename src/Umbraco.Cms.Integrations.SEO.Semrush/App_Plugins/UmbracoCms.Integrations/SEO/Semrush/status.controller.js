(() => {

    function statusController($scope, $window, umbracoCmsIntegrationsSemrushResource) {

        var vm = this;

        vm.isAuthorized = $scope.model.isAuthorized;
        vm.isFreeAccount = $scope.model.isFreeAccount !== null && $scope.model.isAuthorized
            ? ($scope.model.isFreeAccount === true ? "Free" : "Paid") : "N.A.";

        umbracoCmsIntegrationsSemrushResource.getTokenDetails().then(function (response) {

            vm.isAccessTokenAvailable = response.isAccessTokenAvailable;
            vm.AccessToken = response.access_token;

        });

        vm.onRevokeToken = function() {
            vm.revoke();
        }

        vm.revoke = function() {
            if ($scope.model.revoke) {
                $scope.model.revoke();
            }
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