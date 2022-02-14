angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.FormPickerController",
        function ($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmHubspotService, umbracoCmsIntegrationsCrmHubspotResource) {

            const oauthName = "OAuth";

            var vm = this;
            vm.loading = true;
            vm.hubspotFormsList = [];
            vm.searchTerm = "";
            vm.error = "";
            vm.isValid = true;

            // check configuration
            checkConfiguration(loadForms);

            vm.remove = function () {
                $scope.model.value = null;
            };

            vm.saveForm = function (form) {
                $scope.model.value = form;
            };

            vm.openHubspotFormPickerOverlay = function () {
                var options = {
                    title: "HubSpot forms",
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

            function checkConfiguration(callback) {
                umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration().then(function (response) {

                    vm.status = {
                        isValid: response.isValid === true,
                        type: response.type,
                        description: umbracoCmsIntegrationsCrmHubspotService.configDescription[response.type.value],
                        useOAuth: response.isValid === true && response.type.value === oauthName
                    };

                    if (response.isValid === false) {
                        vm.loading = false;
                        vm.error = umbracoCmsIntegrationsCrmHubspotService.configDescription.None;
                        notificationsService.warning("HubSpot API", umbracoCmsIntegrationsCrmHubspotService.configDescription.None);
                    } else {
                        callback();
                    }
                });
            }

            function loadForms() {
                if (vm.status.useOAuth === true) {
                    // use OAuth
                    umbracoCmsIntegrationsCrmHubspotResource.validateAccessToken().then(function (response) {
                        if (response.isExpired === true || response.isValid === false) {
                            vm.loading = false;
                            notificationsService.warning("HubSpot API", "Unable to connect to HubSpot. Please review the settings of the form picker property's data type.");
                            return;
                        }

                        umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsListOAuth().then(function (data) {
                            vm.loading = false;
                            vm.hubspotFormsList = data.forms;

                            if (data.isValid === false || data.isExpired === true) {
                                notificationsService.error("HubSpot API", "Unable to retrieve the list of forms from HubSpot. Please review the settings of the form picker property's data type.");
                            }
                        });
                    });
                } else {
                    // use API
                    umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsList().then(function (data) {
                        vm.loading = false;

                        vm.hubspotFormsList = data.forms;

                        if (data.isValid === false || data.isExpired === true) {
                            notificationsService.error("HubSpot API", "Invalid API key");
                        }
                    });

                }
            }
        });