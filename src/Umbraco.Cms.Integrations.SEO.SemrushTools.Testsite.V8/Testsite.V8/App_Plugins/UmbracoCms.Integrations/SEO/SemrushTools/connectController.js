(() => {

    function connectController($scope, $location) {

        var vm = this;

        vm.close = function() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        }

    }

    angular.module('umbraco')
        .controller('UmbracoCms.Integrations.ConnectController', connectController);

})(); 