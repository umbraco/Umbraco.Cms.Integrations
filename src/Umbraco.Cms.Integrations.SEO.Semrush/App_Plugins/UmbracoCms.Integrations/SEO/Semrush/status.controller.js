﻿(() => {

    function statusController($scope, $window, umbracoCmsIntegrationsSemrushResource) {

        var vm = this;

        umbracoCmsIntegrationsSemrushResource.getTokenDetails().then(function (response) {

            vm.isAccessTokenAvailable = response.isAccessTokenAvailable;
            vm.AccessToken = response.access_token;

        });

        vm.onRevokeToken = function() {
            umbracoCmsIntegrationsSemrushResource.revokeToken().then(function () {

                vm.revoke();

                //vm.close();
            });
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