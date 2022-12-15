function entityPickerEditorController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    vm.entities = [];

    query($scope.model.configuration.entityType);

    function query(entityTypeId) {
        umbracoCmsIntegrationsPimInriverResource.query(entityTypeId).then(function (response) {
            if (response.success) {
                vm.entities = [];
                for (var i = 0; i < response.data.entityIds.length; i++) {
                    umbracoCmsIntegrationsPimInriverResource.getEntitySummary(response.data.entityIds[i]).then(function (summaryResponse) {
                        var entity = {
                            id: summaryResponse.data.id,
                            displayName: summaryResponse.data.displayName,
                            description: summaryResponse.data.description,
                            displayFields: summaryResponse.data.fields.filter(obj => {
                                var index = $scope.model.configuration.displayFieldTypeIds.indexOf(obj.fieldTypeId);
                                if (index > -1) return obj;
                            })
                        };
                        vm.entities.push(entity);
                    });
                }
            }
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerEditorController", entityPickerEditorController);