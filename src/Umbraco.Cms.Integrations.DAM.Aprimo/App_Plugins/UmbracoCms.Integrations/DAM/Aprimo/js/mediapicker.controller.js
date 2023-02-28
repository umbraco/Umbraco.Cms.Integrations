function mediaPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsDamAprimoService, umbracoCmsIntegrationsDamAprimoResource) {

    var vm = this;

    vm.selectedRecord = null;
;
    vm.error = "";

    umbracoCmsIntegrationsDamAprimoResource.checkApiConfiguration().then(function (response) {
        if (response.success) {
            if ($scope.model.value != null && $scope.model.value.length > 0) {
                umbracoCmsIntegrationsDamAprimoResource.getRecordDetails($scope.model.value).then(function (recordResponse) {
                    if (recordResponse.success) {
                        vm.selectedRecord = {
                            title: recordResponse.data.title,
                            extension: recordResponse.data.thumbnail.extension,
                            uri: recordResponse.data.thumbnail.uri
                        };
                    } else
                        notificationsService.error("Aprimo", recordResponse.error);
                });
            }
        } else {
            vm.error = response.error;
        }
    });

    vm.openAprimoMediaPickerOverlay = function () {
        var options = {
            title: "Aprimo Content API",
            subtitle: "Please select an asset",
            configuration: $scope.model.config.configuration,
            view: "/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/mediapickereditor.html",
            size: "medium",
            save: function (assetObj) {
                vm.saveAsset(assetObj);

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    vm.saveAsset = function (id) {
        $scope.model.value = id;

        umbracoCmsIntegrationsDamAprimoResource.getRecordDetails(id).then(function (recordResponse) {
            if (recordResponse.success) {
                vm.selectedRecord = {
                    title: recordResponse.data.title,
                    extension: recordResponse.data.thumbnail.extension,
                    uri: recordResponse.data.thumbnail.uri
                };
            } else
                notificationsService.error("Aprimo", recordResponse.error);
        });
    }

    vm.removeAsset = function () {
        $scope.model.value = null;
        vm.selectedRecord = null;
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.MediaPickerController", mediaPickerController);