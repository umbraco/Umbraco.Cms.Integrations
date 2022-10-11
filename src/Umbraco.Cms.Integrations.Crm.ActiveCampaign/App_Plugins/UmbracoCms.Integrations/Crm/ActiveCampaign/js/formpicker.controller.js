function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmActiveCampaignResource) {

    var vm = this;

    vm.loading = false;
    vm.searchTerm = "";

    vm.selectedForm = {};

    vm.status = "";

    umbracoCmsIntegrationsCrmActiveCampaignResource.checkApiAccess().then(function (response) {
        vm.isApiConfigurationValid = response.isApiConfigurationValid;
        if (response.isApiConfigurationValid) {
            loadForms();
        }
        else {
            vm.status = "Invalid API configuration.";
        }
    });

    vm.saveForm = function (form) {
        $scope.model.value = form;
    }

    vm.removeForm = function () {
        $scope.model.value = null;
    }

    vm.openActiveCampaignFormPickerOverlay = function () {

        var options = {
            title: "ActiveCampaign Forms",
            subtitle: "Select a form",
            view: "/App_Plugins/UmbracoCms.Integrations/Crm/ActiveCampaign/views/formpickereditor.html",
            size: "medium",
            selectForm: function (form) {

                if (form.id !== undefined) {
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
        umbracoCmsIntegrationsCrmActiveCampaignResource.getForms().then(function (response) {
            vm.formsList = [];
            console.log(response);
            if (response) {
                response.forEach(item => {
                    vm.formsList.push({
                        id: item.id,
                        name: item.name
                    });
                });
            }
            vm.loading = false;

            console.log(vm.formsList);
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.ActiveCampaign.FormPickerController", formPickerController);