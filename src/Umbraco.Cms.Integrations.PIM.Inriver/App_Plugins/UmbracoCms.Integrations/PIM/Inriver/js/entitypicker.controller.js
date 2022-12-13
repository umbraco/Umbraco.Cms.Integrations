function entityPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;
    vm.error = '';

    const dialog = document.getElementById('editorDialog');
    dialog.style.display = "block";

    // add event listeners
    var btnCancel = dialog.querySelector('#btnCancel');
    btnCancel.addEventListener('click', closeEditor);


    umbracoCmsIntegrationsPimInriverResource.checkApiAccess().then(function (response) {
        if (response.failure) 
            vm.error = response.data;
    });

    vm.openInriverEntityPickerOverlay = function () {
        var options = {
            title: "Inriver Entities",
            subtitle: "Select entity",
            configuration: {
                entityType: $scope.model.config.configuration.EntityType,
                allowChange: $scope.model.config.configuration.AllowChange
            },
            view: "/App_Plugins/UmbracoCms.Integrations/PIM/Inriver/views/entitypickereditor.html",
            size: "medium",
            select: function (entity) {
                //vm.saveForm(form);

                //editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    function closeEditor() {
        const dialog = document.getElementById('editorDialog');
        dialog.style.display = "none";
    }

    //vm.saveForm = function (formId) {
    //    $scope.model.value = formId;

    //    getFormDetails(formId);
    //}

    //vm.removeForm = function () {
    //    $scope.model.value = null;
    //    vm.selectedForm = {};
    //}

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerController", entityPickerController);