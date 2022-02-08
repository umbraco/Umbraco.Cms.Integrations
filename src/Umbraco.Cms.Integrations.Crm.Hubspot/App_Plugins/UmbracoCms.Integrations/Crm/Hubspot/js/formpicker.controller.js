angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.FormPickerController",
        function ($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {

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

            function checkConfiguration(callback) {
                umbracoCmsIntegrationsCrmHubspotResource.checkApiConfiguration().then(function (response) {

                    vm.status = {
                        isValid: response.isValid === true,
                        type: response.type,
                        description: response.isValid === true ? `${response.type.value} is configured.` : "Invalid configuration",
                        useOAuth: response.isValid === true && response.type.value === oauthName
                    };

                    if (response.isValid === false) {
                        vm.loading = false;
                        vm.error = "Invalid configuration.";
                        notificationsService.warning("Hubspot API",
                            "Invalid setup. Please review the API/OAuth settings.");
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
                            notificationsService.warning("Hubspot API", "Invalid Access Token");
                            return;
                        }

                        umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsListOAuth().then(function (data) {
                            vm.loading = false;
                            vm.hubspotFormsList = data.forms;

                            if (data.isValid === false || data.isExpired === true) {
                                notificationsService.error("Hubspot API", "Invalid Access Token");
                            }
                        });
                    });
                } else {
                    // use API
                    umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsList().then(function (data) {
                        vm.loading = false;

                        vm.hubspotFormsList = data.forms;

                        if (data.isValid === false || data.isExpired == true) {
                            notificationsService.error("Hubspot API", "Invalid API key");
                        }
                    });

                }
            }
        });