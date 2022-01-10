angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.FormPickerController",
        function ($scope, hubspotResources) {
            var vm = this;
            vm.loading = true;
            vm.hubspotFormsList = [];
            vm.searchTerm = "";
            vm.error = "";
            
            hubspotResources.getHubspotFormsList().then(function (data) {
                vm.hubspotFormsList = data;
                vm.loading = false;
               //errorcheck
               console.log(data);
            });

            vm.remove = function() {
                $scope.model.value = null;
            };

            vm.saveForm = function(form) {
                $scope.model.value = form;
            };

            vm.openHubspotFormPickerOverlay = function () {
                vm.hubspotFormPickerOverlay = {
                    view: "/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpickeroverlay.html",
                    show: true,
                    title: "Hubspot forms",
                    subtitle: "Select a form",
                    hideSubmitButton: true,
                    pickForm: function (form) {
                        vm.saveForm(form);
                        vm.hubspotFormPickerOverlay.show = false;
                        vm.hubspotFormPickerOverlay = null;
                    },
                    close: function () {
                        vm.hubspotFormPickerOverlay.show = false;
                        vm.hubspotFormPickerOverlay = null;
                    }
                };
            };
        });