function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmActiveCampaignResource) {

    var vm = this;

    vm.loading = false;
    vm.searchTerm = "";

    vm.selectedForm = {};

    vm.status = "";

    umbracoCmsIntegrationsCrmActiveCampaignResource.checkApiAccess().then(function (response) {
        vm.isApiConfigurationValid = response.isApiConfigurationValid;
        if (response.isApiConfigurationValid) {
            if ($scope.model.value) {
                getFormDetails($scope.model.value);
            }

            loadForms();
        }
        else {
            vm.status = "Invalid API configuration.";
        }
    });

    vm.saveForm = function (formId) {
        $scope.model.value = formId;

        getFormDetails(formId);
    }

    vm.removeForm = function () {
        $scope.model.value = null;
        vm.selectedForm = {};
    }

    vm.openActiveCampaignFormPickerOverlay = function () {

        var options = {
            title: "ActiveCampaign Forms",
            subtitle: "Select a form",
            view: "/App_Plugins/UmbracoCms.Integrations/Crm/ActiveCampaign/views/formpickereditor.html",
            size: "medium",
            selectForm: function (form) {

                if (form.id !== undefined) {
                    vm.saveForm(form.id);
                }

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    };

    function getFormDetails(id) {
        vm.loading = true;
        umbracoCmsIntegrationsCrmActiveCampaignResource.getForm(id).then(function (response) {
            if (response.message && response.message.length > 0)
                vm.status = response.message;
            else
                vm.selectedForm = response.form;

            vm.loading = false;
        });
    }

    function loadForms() {
        vm.loading = true;
        umbracoCmsIntegrationsCrmActiveCampaignResource.getForms().then(function (response) {
            vm.formsList = [];

            if (response.forms != null) {
                response.forms.forEach(item => {
                    vm.formsList.push({
                        id: item.id,
                        name: item.name
                    });
                });
            }
            else vm.status = response.message;

            vm.loading = false;
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.ActiveCampaign.FormPickerController", formPickerController);