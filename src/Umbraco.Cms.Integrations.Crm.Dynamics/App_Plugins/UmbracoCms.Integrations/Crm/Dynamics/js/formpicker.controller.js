function formPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {
    var vm = this;

    vm.loading = false;
    vm.dynamicsFormsList = [];
    vm.searchTerm = "";

    loadForms();

    vm.openDynamicsFormPickerOverlay = function () {

        var options = {
            title: "Dynamics - Marketing Forms",
            subtitle: "Select a form",
            view: "/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/formpickereditor.html",
            size: "medium",
            selectForm: function (form) {
                //vm.saveForm(form);

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
                        name: item.msdyncrm_name
                    });
                });
                console.log(vm.dynamicsFormsList);
            }
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Dynamics.FormPickerController", formPickerController);