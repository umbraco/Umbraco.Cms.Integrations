function entityPickerEditorController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    umbracoCmsIntegrationsPimInriverResource.getEntityTypes().then(function (entityTypesResponse) {
        var entityTypes = entityTypesResponse.data.map(obj => {
            var option = {
                name: obj.name,
                value: obj.id
            };
            if ($scope.model.configuration.entityType == obj.id) {
                option.selected = true;
            }
            return option;
        });
        bindValues(entityTypes);
    });

    function bindValues(entityTypes) {
        var selEntityTypes = document.getElementById("selEntityTypes");
        selEntityTypes.options = entityTypes;

        var selectedEntityType = entityTypes.find(obj => obj.selected);
        query(selectedEntityType.value);
    }

    function query(entityTypeId) {
        umbracoCmsIntegrationsPimInriverResource.query(entityTypeId).then(function (response) {
            if (response.success) {
                vm.entities = [];
                for (var i = 0; i < response.data.entityIds.length; i++) {
                    umbracoCmsIntegrationsPimInriverResource.getEntitySummary(response.data.entityIds[i]).then(function (summaryResponse) {
                        vm.entities.push(summaryResponse.data);
                    });
                }
            }
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerEditorController", entityPickerEditorController);