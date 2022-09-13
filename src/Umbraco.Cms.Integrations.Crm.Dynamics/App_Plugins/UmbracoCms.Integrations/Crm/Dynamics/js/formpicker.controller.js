function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {

    var vm = this;

    vm.loading = false;
    vm.searchTerm = "";
    vm.selectedForm = {};
    vm.iframeEmbedded = false;
    vm.isConnected = false;

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

            if (response.length == 0) {
                notificationsService.warning("Dynamics API", "Unable to embed selected form. Please check if it is live.");
            }

            form.embedCode = response;

            $scope.model.value = form;
        });
    }

    vm.removeForm = function () {
        $scope.model.value = null;
    }

    vm.toggleRenderMode = function () {
        vm.iframeEmbedded = !vm.iframeEmbedded;
    }

    vm.openDynamicsFormPickerOverlay = function () {

        var options = {
            title: "Dynamics - Marketing Forms",
            subtitle: "Select a form",
            view: "/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/formpickereditor.html",
            size: "medium",
            selectForm: function (form, iframeEmbedded) {

                if (form.id !== undefined) {
                    form.iframeEmbedded = iframeEmbedded;

                    vm.saveForm(form);
                }

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    function loadForms() {
        vm.loading = true;
        umbracoCmsIntegrationsCrmDynamicsResource.getForms().then(function (response) {
            vm.dynamicsFormsList = [];
            if (response) {
                response.value.forEach(item => {
                    vm.dynamicsFormsList.push({
                        id: item.msdyncrm_marketingformid,
                        name: item.msdyncrm_name,
                        embedCode: "",
                        iframeEmbedded: vm.iframeEmbedded
                    });
                });
            }
            vm.loading = false;
        });
    }

    $scope.connected = function () {
        vm.isConnected = true;
        loadForms();
    };

    $scope.revoked = function () {
        vm.isConnected = false;
        vm.dynamicsFormsList = [];
    };
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Dynamics.FormPickerController", formPickerController);