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
            vm.euRegion = false;

            const configDescription = {
                API: "An API key is configured and will be used to connect to your HubSpot account.",
                OAuth:
                    "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
                None: "No API or OAuth configuration could be found. Please review your settings before continuing.",
                OAuthConnected:
                    "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'"
            };

            // check configuration
            checkConfiguration(loadForms);

            vm.toggleRegion = function() {
                vm.euRegion = !vm.euRegion;
            }

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
                    pickForm: function (form, euRegion) {

                        form.euRegion = euRegion;

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
                        description: configDescription[response.type.value],
                        useOAuth: response.isValid === true && response.type.value === oauthName
                    };

                    if (response.isValid === false) {
                        vm.loading = false;
                        vm.error = configDescription.None;
                        notificationsService.warning("Hubspot API", configDescription.None);
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
                            notificationsService.warning("Hubspot API", "Invalid access token. Please review OAuth settings of the editor.");
                            return;
                        }

                        umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsListOAuth().then(function (data) {
                            vm.loading = false;
                            vm.hubspotFormsList = data.forms;

                            if (data.isValid === false || data.isExpired === true) {
                                notificationsService.error("Hubspot API", "Invalid access token. Please review OAuth settings of the editor.");
                            }
                        });
                    });
                } else {
                    // use API
                    umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsList().then(function (data) {
                        vm.loading = false;

                        vm.hubspotFormsList = data.forms;

                        if (data.isValid === false || data.isExpired === true) {
                            notificationsService.error("Hubspot API", "Invalid API key");
                        }
                    });

                }
            }
        });