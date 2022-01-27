angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Hubspot.FormPickerController",
        function ($scope, editorService, notificationsService, umbracoCmsIntegrationsCrmHubspotResource) {
            var vm = this;
            vm.loading = true;
            vm.hubspotFormsList = [];
            vm.searchTerm = "";
            vm.error = "";

            if ($scope.model.config !== undefined && $scope.model.config.settings !== undefined) {
                vm.config = {
                    useApi: $scope.model.config.settings.useApi,
                    useOAuth: $scope.model.config.settings.useOAuth,
                    isOverlay: false
                };
            }
            if ($scope.model.overlayConfig !== undefined && $scope.model.overlayConfig.isOverlay !== undefined) {
                vm.config = {
                    useApi: $scope.model.overlayConfig.useApi,
                    useOAuth: $scope.model.overlayConfig.useOAuth,
                    isOverlay: true
                };
            }

            if (vm.config !== undefined && vm.config.useApi === false && vm.config.useOAuth === false) {
                vm.loading = false;
                notificationsService.warning("Authorization", "No authorization setup has been selected");
                return;
            } 

            if (vm.config.useApi === true) {
                umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsList().then(function (data) {
                    vm.loading = false;

                    vm.hubspotFormsList = data.forms;

                    if (data.isValid === false || data.isExpired == true) {
                        notificationsService.error("Hubspot API", "Invalid API key");
                    }
                });
            } else if (vm.config.useOAuth === true) {
                umbracoCmsIntegrationsCrmHubspotResource.validateAccessToken().then(function(response) {
                    if (response.isExpired === true || response.isValid === false) {
                        notificationsService.warning("Hubspot API", "Invalid Access Token");
                        return;
                    }

                    umbracoCmsIntegrationsCrmHubspotResource.getHubspotFormsListOAuth().then(function(data) {
                        vm.loading = false;
                        vm.hubspotFormsList = data.forms;

                        if (data.isValid === false || data.isExpired == true) {
                            notificationsService.error("Hubspot API", "Invalid Access Token");
                        }
                    });
                });
            }
 
            vm.remove = function () {
                $scope.model.value = null;
            };

            vm.saveForm = function (form) {
                $scope.model.value = form;
            };

            vm.openHubspotFormPickerOverlay = function () {
                vm.config.isOverlay = true;
                var options = {
                    title: "Hubspot forms",
                    subtitle: "Select a form",
                    view: "/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/views/formpickereditor.html",
                    size: "medium",
                    overlayConfig: vm.config,
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