function configurationController($scope, notificationsService, umbracoCmsIntegrationsPimInriverService, umbracoCmsIntegrationsPimInriverResource) {
    var vm = this;

    const selEntityTypes = document.getElementById("selEntityTypes");

    vm.configuration = {};

    // object for populating configuration data
    vm.data = {
        entityTypes: [],
        fieldTypes: [],
        linkedTypes: []
    };

    // object for selected data
    vm.selectedData = {
        entityType: {},
        fieldTypes: [],
        linkedTypes: []
    };

    if ($scope.model.value == null) {
        $scope.model.value = {
            entityType: '',
            fieldTypes: [],
            linkedTypes: []
        };
    }
    $scope.$on('formSubmitting', function (ev) {
        if (vm.selectedData.entityType == undefined
            || vm.selectedData.entityType.length == 0
            || vm.selectedData.fieldTypes.length == 0) {
            notificationsService.error("Inriver", "Entity type and display fields are required. Configuration was not saved.");
            ev.preventDefault();
            return;
        } else {
            $scope.model.value = {
                entityType: vm.selectedData.entityType.value,
                fieldTypes: vm.selectedData.fieldTypes,
                linkedTypes: vm.selectedData.linkedTypes
            };
        }
    });

    vm.entityTypeChange = function () {
        vm.selectedData.entityType = vm.entityTypes.find(obj => obj.value == selEntityTypes.value);;

        vm.data.fieldTypes = vm.selectedData.entityType.fieldTypes;
        vm.data.linkedTypes = vm.selectedData.entityType.linkedTypes;

        vm.selectedData.fieldTypes = [];
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
                        fieldTypes: obj.fieldTypes,
                        linkedTypes: obj.outboundLinkTypes
                    };
                    if ($scope.model.value !== null && $scope.model.value.entityType == obj.id) {
                        option.selected = true;

                        vm.selectedData.entityType = option;
                    }
                    return option;
                });

                if ($scope.model.value.fieldTypes != null)
                    vm.selectedData.fieldTypes = $scope.model.value.fieldTypes;

                if ($scope.model.value.linkedTypes != null)
                    vm.selectedData.linkedTypes = $scope.model.value.linkedTypes;

                bindValues();
            });

        }
    });

    // table rows selection
    vm.selectFieldType = function (fieldType) {
        vm.selectedData.fieldTypes.push(fieldType);
    }

    vm.unselectFieldType = function (fieldTypeId) {
        vm.selectedData.fieldTypes = vm.selectedData.fieldTypes.filter(obj => obj.fieldTypeId != fieldTypeId);
    }

    vm.toggleLinkedType = function (linkedType) {
        const chkEl = document.getElementById("chk" + linkedType);

        if (chkEl.checked)
            vm.selectedData.linkedTypes.push(linkedType);
        else
            vm.selectedData.linkedTypes = vm.selectedData.linkedTypes.filter(obj => obj != linkedType);
    }

    function bindValues() {
        selEntityTypes.options = vm.entityTypes;
        vm.data.fieldTypes = vm.selectedData.entityType.fieldTypes;
        vm.data.linkedTypes = vm.selectedData.entityType.linkedTypes;

        // select field types
        if ($scope.model.value.fieldTypes != null) {
            $scope.model.value.fieldTypes.forEach(obj => {
                umbracoCmsIntegrationsPimInriverService.waitForElement("#tr" + obj.fieldTypeId)
                    .then(element => element.setAttribute("selected", ""));
            });
        }

        // select linked types
        if ($scope.model.value.linkedTypes != null) {
            $scope.model.value.linkedTypes.forEach(obj => {
                umbracoCmsIntegrationsPimInriverService.waitForElement("#chk" + obj)
                    .then(element => element.setAttribute("checked", ""));
            });
        }
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.ConfigurationController", configurationController);