﻿function entityPickerController($scope, editorService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    vm.selectedEntity = null;
    vm.error = "";
    vm.configuration = $scope.model.config.configuration;

    umbracoCmsIntegrationsPimInriverResource.checkApiAccess().then(function (response) {
        if (response.failure) 
            vm.error = response.data;
    });
    
    if (vm.configuration.entityType == undefined
        || vm.configuration.entityType.length == 0
        || vm.configuration.displayFieldTypeIds == null
        || vm.configuration.displayFieldTypeIds.length == 0) {
        vm.error = "Invalid Inriver configuration";
        return;
    }

    if ($scope.model.value) {
        getEntityData($scope.model.value.entityId);
    }

    vm.openInriverEntityPickerOverlay = function () {
        var options = {
            title: "Inriver Entities",
            subtitle: "Select entity",
            configuration: {
                entityType: $scope.model.config.configuration.entityType,
                displayFieldTypeIds: $scope.model.config.configuration.displayFieldTypeIds
            },
            view: "/App_Plugins/UmbracoCms.Integrations/PIM/Inriver/views/entitypickereditor.html",
            size: "medium",
            save: function (entityId) {
                vm.saveEntity(entityId);
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };
    
    vm.saveEntity = function (entityId) {
        $scope.model.value = JSON.stringify({
            entityId: entityId,
            displayFields: $scope.model.config.configuration.displayFieldTypeIds
        });

        getEntityData(entityId);
    }

    vm.removeEntity = function () {
        $scope.model.value = null;
        vm.selectedEntity = null;
    }

    function getEntityData(entityId) {
        umbracoCmsIntegrationsPimInriverResource.fetchEntityData(entityId).then(function (response) {
            if (response.success) {
                vm.selectedEntity = response.data[0];
                vm.selectedEntity.detail = $scope.model.config.configuration.displayFieldTypeIds.join(",");
            } else
                vm.error = response.error;
        });
    }

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerController", entityPickerController);