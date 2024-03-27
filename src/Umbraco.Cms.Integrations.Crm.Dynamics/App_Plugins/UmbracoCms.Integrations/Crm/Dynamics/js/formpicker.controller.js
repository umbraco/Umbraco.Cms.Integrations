function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {

    var vm = this;

    vm.loading = false;
    vm.searchTerm = "";
    vm.selectedForm = {};
    vm.iframeEmbedded = false;
    vm.isConnected = false;
    vm.formsLoading = true;

    umbracoCmsIntegrationsCrmDynamicsResource.checkOAuthConfiguration().then(function (response) {
        if (!response.isAuthorized) {
            let error = "Unable to connect to Dynamics. Please review the settings of the form picker property's data type.";
            notificationsService.error("Dynamics API", error);
            vm.error = error;
        }
    });

    vm.saveForm = function (form) {

        if (form.iframeEmbedded) {
            umbracoCmsIntegrationsCrmDynamicsResource.getEmbedCode(form.id).then(function (response) {

                if (response.length == 0) {
                    notificationsService.warning("Dynamics API", "Unable to embed selected form. Please check if it is live.");
                }

                form.embedCode = response;
            });
        }

        $scope.model.value = form;
    }

    vm.removeForm = function () {
        $scope.model.value = null;
    }

    vm.toggleRenderMode = function () {
        vm.iframeEmbedded = !vm.iframeEmbedded;
    }

    vm.openDynamicsFormPickerOverlay = function () {

        var module = $scope.model.config.module === "Both"
            ? "Outbound | Real-Time"
            : $scope.model.config.module;
        var options = {
            title: `Dynamics - ${module} Marketing Forms`,
            subtitle: "Select a form",
            module: $scope.model.config.module,
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
        vm.formsLoading = true;
        umbracoCmsIntegrationsCrmDynamicsResource.getForms($scope.model.module).then(function (response) {
            vm.dynamicsFormsList = [];
            if (response && response.length > 0) {
                response.forEach(item => {
                    vm.dynamicsFormsList.push({
                        id: item.id,
                        name: item.name,
                        module: item.module
                    });
                });
            }

        }).finally(function () {
            vm.loading = false;
            vm.formsLoading = false;
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