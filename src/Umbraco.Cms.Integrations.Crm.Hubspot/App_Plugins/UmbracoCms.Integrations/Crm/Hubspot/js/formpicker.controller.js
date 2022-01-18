angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.FormPickerController",
        function ($scope, editorService, umbracoCmsIntegrationsCrmHubspotResource) {
            var vm = this;
            vm.loading = true;
            vm.hubspotFormsList = [];
            vm.searchTerm = "";
            vm.error = "";

            umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsList().then(function (data) {
                vm.hubspotFormsList = data;
                vm.loading = false;

                //errorcheck
                console.log(data);
            });

            vm.remove = function () {
                $scope.model.value = null;
            };

            vm.saveForm = function (form) {
                $scope.model.value = form;
            };

            vm.openHubspotFormPickerOverlay = function () {

                var options = {
                    title: "Hubspot forms",
                    subtitle: "Select a form",
                    view: "/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpickereditor.html",
                    size: "medium",
                    pickForm: function (form) {
                        vm.saveForm(form);
                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
                };

                editorService.open(options);
            };
        });