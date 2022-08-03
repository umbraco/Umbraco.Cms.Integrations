function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {

    var vm = this;

    vm.loading = false;
    vm.dynamicsFormsList = [];
    vm.searchTerm = "";
    vm.iFrameEmbedded = false;

    umbracoCmsIntegrationsCrmDynamicsResource.checkOAuthConfiguration().then(function (response) {
        if (response.isAuthorized) {
            loadForms();
        } else {
            let error = "Unable to connect to Dynamics. Please review the settings of the form picker property's data type.";
            notificationsService.error("Dynamics API", error);
            vm.error = error;
        }
    });

    vm.saveForm = function (form) {

        umbracoCmsIntegrationsCrmDynamicsResource.getEmbedCode(form.id).then(function (response) {

            form.embedCode = response;

            $scope.model.value = form;
        });
    }

    vm.removeForm = function () {
        $scope.model.value = null;
    }

    vm.toggleRenderMode = function () {
        vm.iFrameEmbedded = !vm.iFrameEmbedded;
    }

    vm.openDynamicsFormPickerOverlay = function () {

        var options = {
            title: "Dynamics - Marketing Forms",
            subtitle: "Select a form",
            view: "/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/formpickereditor.html",
            size: "medium",
            selectForm: function (form, iFrameEmbedded) {

                form.iFrameEmbedded = iFrameEmbedded;

                vm.saveForm(form);

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    function loadForms() {
        umbracoCmsIntegrationsCrmDynamicsResource.getForms().then(function (response) {
            if (response) {
                response.value.forEach(item => {
                    vm.dynamicsFormsList.push({
                        id: item.msdyncrm_marketingformid,
                        name: item.msdyncrm_name,
                        embedCode: "",
                        iFrameEmbedded: vm.iFrameEmbedded
                    });
                });
            }
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Dynamics.FormPickerController", formPickerController);