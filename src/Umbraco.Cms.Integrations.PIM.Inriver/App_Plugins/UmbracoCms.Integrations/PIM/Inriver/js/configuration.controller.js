function configurationController($scope, notificationsService, umbracoCmsIntegrationsPimInriverService, umbracoCmsIntegrationsPimInriverResource) {
    var vm = this;

    const selEntityTypes = document.getElementById("selEntityTypes");

    vm.configuration = {};

    vm.entityTypes = [];
    vm.fieldTypes = [];

    vm.selectedEntityType = {};
    vm.selectedFieldTypes = [];

    vm.showToast = showToast;

    //vm.showToast({
    //    color: 'danger',
    //    headline: 'Algolia',
    //    message: 'Index name and content schema are required.'
    //});

    if ($scope.model.value == null) {
        $scope.model.value = {
            entityType: '',
            displayFieldTypeIds: []
        };
    }
    $scope.$on('formSubmitting', function (ev) {
        if (vm.selectedEntityType != undefined
            || vm.selectedEntityType.value.length == 0
            || vm.selectedFieldTypes.length == 0) {
            notificationsService.error("Inriver", "Entity type and display fields are required. Configuration was not saved.");
            ev.preventDefault();
            return;
        } else {
            $scope.model.value.entityType = vm.selectedEntityType.value;
            $scope.model.value.displayFieldTypeIds = vm.selectedFieldTypes;
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

                if ($scope.model.value.displayFieldTypeIds != null)
                    vm.selectedFieldTypes = $scope.model.value.displayFieldTypeIds;

                bindValues();
            });

        }
    });

    // table rows selection
    vm.selectFieldType = function (fieldTypeId) {
        vm.selectedFieldTypes.push(fieldTypeId);
    }

    vm.unselectFieldType = function (fieldTypeId) {
        vm.selectedFieldTypes = vm.selectedFieldTypes.filter(id => fieldTypeId != id);
    }

    function bindValues() {
        selEntityTypes.options = vm.entityTypes;
        vm.fieldTypes = vm.selectedEntityType.fieldTypes;

        if ($scope.model.value.displayFieldTypeIds != null) {
            $scope.model.value.displayFieldTypeIds.forEach(fieldTypeId => {
                umbracoCmsIntegrationsPimInriverService.waitForElement("#tr" + fieldTypeId)
                    .then(element => element.setAttribute("selected", ""));
            });
        }
    }

    /* Toast Config properties:
     *  color
     *  headline
     *  message
     */
    function showToast(config) {
        const con = document.querySelector('uui-toast-notification-container');

        const toast = document.createElement('uui-toast-notification');
        toast.color = config.color;

        const toastLayout = document.createElement('uui-toast-notification-layout');
        toastLayout.headline = config.headline;
        toast.appendChild(toastLayout);

        const messageEl = document.createElement('span');
        messageEl.innerHTML = config.message;
        toastLayout.appendChild(messageEl);

        if (con) {
            con.appendChild(toast);
        }
    }

    /**
     * toggle rows selection with uui-checkbox - prototype
     * */
    vm._selectFieldType = function (fieldTypeId) {
        var fieldTypeIndex = vm.selectedFieldTypes.indexOf(fieldTypeId);
        if (fieldTypeIndex == -1) {
            vm.selectedFieldTypes.push(fieldTypeId);
            document.getElementById('chk' + fieldTypeId).setAttribute('checked', '');
        }
        else {
            document.getElementById('chk' + fieldTypeId).removeAttribute('checked');
            vm.selectedFieldTypes = vm.selectedFieldTypes.splice(fieldTypeIndex, 1);
        }
    }

    vm._selectFieldTypes = function () {
        const selectAll = vm.selectedFieldTypes.length == 0;

        vm.selectedFieldTypes = [];

        console.log('select all', selectAll);
        if (selectAll == true) {
            vm.selectedFieldTypes = vm.fieldTypes.map(obj => obj.fieldTypeId);
            var elements = document.querySelectorAll("uui-checkbox");
            for (var i = 0; i < elements.length; i++) {
                elements[i].setAttribute("checked", "");
            }
        }
        else {
            var elements = document.querySelectorAll("uui-checkbox");
            for (var i = 0; i < elements.length; i++) {
                elements[i].removeAttribute("checked");
            }
        }

        vm.fieldTypes = vm.fieldTypes.map(obj => {
            obj.selected = selectAll;
            return obj;
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.ConfigurationController", configurationController);