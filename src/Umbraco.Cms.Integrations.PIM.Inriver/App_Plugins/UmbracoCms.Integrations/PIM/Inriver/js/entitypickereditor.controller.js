function entityPickerEditorController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    vm.loading = false;
    vm.entities = [];

    vm.pagination = {
        pageNumber: 1,
        itemsPerPage: 3,
        totalPages: 1
    };

    query();

    function query() {

        var entityTypeId = $scope.model.configuration.entityType;
        var fieldTypeIds = $scope.model.configuration.displayFieldTypeIds.join(',');

        vm.loading = true;
        umbracoCmsIntegrationsPimInriverResource.query(entityTypeId, fieldTypeIds).then(function (response) {

            console.log(response);

            //if (response.success) {
            //    let data = [];
            //    for (var i = 0; i < response.data.entityIds.length; i++) {
            //        umbracoCmsIntegrationsPimInriverResource.getEntitySummary(response.data.entityIds[i]).then(function (summaryResponse) {
            //            var entity = {
            //                id: summaryResponse.data.id,
            //                displayName: summaryResponse.data.displayName,
            //                description: summaryResponse.data.description,
            //                displayFields: summaryResponse.data.fields.filter(obj => {
            //                    var index = $scope.model.configuration.displayFieldTypeIds.indexOf(obj.fieldTypeId);
            //                    if (index > -1) return obj;
            //                })
            //            };
            //            //vm.entities.push(entity);
            //        });
            //    }
            //}

            vm.loading = false;
        });
    }

    vm.save = function (entityId) {
        $scope.model.save(entityId);
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerEditorController", entityPickerEditorController);