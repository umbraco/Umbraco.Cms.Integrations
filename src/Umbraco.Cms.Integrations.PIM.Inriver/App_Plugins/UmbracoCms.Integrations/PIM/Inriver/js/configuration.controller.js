function configurationController($scope, notificationsService, umbracoCmsIntegrationsPimInriverService, umbracoCmsIntegrationsPimInriverResource) {
    var vm = this;

    const selEntityTypes = document.getElementById("selEntityTypes");

    vm.configuration = {};

    vm.entityTypes = [];
    vm.fieldTypes = [];

    vm.selectedEntityType = {};
    vm.selectedFieldTypes = [];

    if ($scope.model.value == null) {
        $scope.model.value = {
            entityType: '',
            fieldTypes: []
        };
    }
    $scope.$on('formSubmitting', function (ev) {
        if (vm.selectedEntityType == undefined
            || vm.selectedEntityType.length == 0
            || vm.selectedFieldTypes.length == 0) {
            notificationsService.error("Inriver", "Entity type and display fields are required. Configuration was not saved.");
            ev.preventDefault();
            return;
        } else {
            $scope.model.value.entityType = vm.selectedEntityType.value;
            $scope.model.value.fieldTypes = vm.selectedFieldTypes;
        }
    });

    vm.entityTypeChange = function () {
        var selectedEntityType = vm.entityTypes.find(obj => obj.value == selEntityTypes.value);

        vm.selectedEntityType = selectedEntityType;
        vm.fieldTypes = vm.selectedEntityType.fieldTypes;

        vm.selectedFieldTypes = [];
    }
  
    umbracoCmsIntegrationsPimInriverResource.checkApiAccess().then(function (response) {
        vm.configuration.icon = response.success ? 'unlock' : 'lock';
        vm.configuration.tag = response.success ? 'positive' : 'danger';
        vm.configuration.status = response;

        if (response.success) {
            umbracoCmsIntegrationsPimInriverResource.getEntityTypes().then(function (entityTypesResponse) {
                vm.entityTypes = entityTypesResponse.data.map(obj => {
                    var option = {
                        value: obj.id,
                        name: obj.name,
                        fieldTypes: obj.fieldTypes
                    };
                    if ($scope.model.value !== null && $scope.model.value.entityType == obj.id) {
                        option.selected = true;

                        vm.selectedEntityType = option;
                    }
                    return option;
                });

                if ($scope.model.value.fieldTypes != null)
                    vm.selectedFieldTypes = $scope.model.value.fieldTypes;

                bindValues();
            });

        }
    });

    // table rows selection
    vm.selectFieldType = function (fieldType) {
        vm.selectedFieldTypes.push(fieldType);
    }

    vm.unselectFieldType = function (fieldTypeId) {
        vm.selectedFieldTypes = vm.selectedFieldTypes.filter(obj => obj.fieldTypeId != fieldTypeId);
    }

    function bindValues() {
        selEntityTypes.options = vm.entityTypes;
        vm.fieldTypes = vm.selectedEntityType.fieldTypes;

        if ($scope.model.value.fieldTypes != null) {
            $scope.model.value.fieldTypes.forEach(obj => {
                umbracoCmsIntegrationsPimInriverService.waitForElement("#tr" + obj.fieldTypeId)
                    .then(element => element.setAttribute("selected", ""));
            });
        }
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.ConfigurationController", configurationController);