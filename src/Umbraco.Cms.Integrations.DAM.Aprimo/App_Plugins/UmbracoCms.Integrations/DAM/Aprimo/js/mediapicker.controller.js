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

    vm.selectRecord = () => {
        if ($scope.model.config.configuration.useContentSelector)
            openAprimoContentSelector();
        else
            openAprimoMediaPickerOverlay();
    }

    vm.saveAsset = (id) => {
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

    vm.removeAsset = () => {
        $scope.model.value = null;
        vm.selectedRecord = null;
    }

    function openAprimoMediaPickerOverlay() {
        var options = {
            title: "Aprimo Content API",
            subtitle: "Please select an asset",
            configuration: $scope.model.config.configuration,
            view: "/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/mediapickereditor.html",
            size: "medium",
            save: function (id) {
                vm.saveAsset(id);

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    function openAprimoContentSelector() {
        umbracoCmsIntegrationsDamAprimoResource.getContentSelectorUrl().then(function (response) {
            if (response.length > 0) {
                window.open(response,
                    "Aprimo Content Selector",
                    "width=900,height=700,modal=yes,alwaysRaised=yes");

                window.addEventListener("message", function (event) {
                    if (event.data.result !== 'cancel' && event.data.selection !== undefined) {
                        vm.saveAsset(event.data.selection[0].id);
                    }
                }, false);
            }
            else
                notificationsService.warning("Aprimo", "Could not retrieve content selector URL.");
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.MediaPickerController", mediaPickerController);