function configurationController($scope, umbracoCmsIntegrationsPimInriverResource) {
    var vm = this;

    vm.configuration = {};

    if ($scope.model.value == null) {
        $scope.model.value = {
            entityType: '',
            allowChange: false
        };
    }

    $scope.$on('formSubmitting', function () {
        var selEntityTypes = document.getElementById("selEntityTypes");
        var chkAllowChange = document.getElementById("chkAllowChange");

        $scope.model.value.entityType = selEntityTypes.value;
        $scope.model.value.allowChange = chkAllowChange.checked;
    });

    umbracoCmsIntegrationsPimInriverResource.checkApiAccess().then(function (response) {
        vm.configuration.icon = response.success ? 'unlock' : 'lock';
        vm.configuration.tag = response.success ? 'positive' : 'danger';
        vm.configuration.status = response;

        if (response.success) {
            umbracoCmsIntegrationsPimInriverResource.getEntityTypes().then(function (entityTypesResponse) {
                var entityTypes = entityTypesResponse.data.map(obj => {
                    var option = {
                        name: obj.name,
                        value: obj.id
                    };
                    if ($scope.model.value !== null && $scope.model.value.EntityType == obj.id) {
                        option.selected = true;
                    }
                    return option;
                });
                bindValues(entityTypes);
            });

        }
    });

    function bindValues(entityTypes) {
        var selEntityTypes = document.getElementById("selEntityTypes");
        selEntityTypes.options = entityTypes;

        var chkAllowChange = document.getElementById("chkAllowChange");
        chkAllowChange.checked = $scope.model.value.AllowChange;
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.ConfigurationController", configurationController);