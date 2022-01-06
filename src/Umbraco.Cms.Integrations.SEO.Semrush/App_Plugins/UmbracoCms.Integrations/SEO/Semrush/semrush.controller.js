(function () {
    angular.module("umbSemrushModule", []);

    var umbracoApp = angular.module("umbraco");
    umbracoApp.requires.push("umbSemrushModule");

    angular.module("umbSemrushModule")
        .constant("umbPropertyTypeAlias",
            {
                "TEXTBOX": "textbox",
                "TEXTAREA": "textarea"
            });
})();

(() => {

    function semrushController($scope, $window, editorState, notificationsService, editorService, contentResource,
        umbracoCmsIntegrationsSemrushResource, umbPropertyTypeAlias) {

        var vm = this;

        vm.searchKeywordsBoxVisible = false;
        vm.isConnected = false;
        vm.loading = false;

        vm.searchQuery = '';

        vm.dataSourceItems = [];
        vm.supportedMethods = [
            {
                "Key": "phrase_fullsearch",
                "Value": "phrase_fullsearch"
            },
            {
                "Key": "phrase_kdl",
                "Value": "phrase_kdl"
            },
            {
                "Key": "phrase_organic",
                "Value": "phrase_organic"
            },
            {
                "Key": "phrase_related",
                "Value": "phrase_related"
            },
            {
                "Key": "phrase_these",
                "Value": "phrase_these"
            },
            {
                "Key": "phrase_this",
                "Value": "phrase_this"
            }
        ];
        vm.relatedPhrasesList = {};
        vm.currentNodeProperties = [];

        vm.pagination = {
            pageNumber: 1,
            totalPages: 1
        };
        vm.nextPage = nextPage;
        vm.prevPage = prevPage;
        vm.changePage = changePage;
        vm.goToPage = goToPage;

        // content handling
        vm.currentNodeId = editorState.current.id;
        vm.currentNodeAlias = editorState.current.contentTypeAlias;

        var currentVariant = editorState.current.variants.find(x => x.active === true);
        var tabs = currentVariant.tabs;

        for (var tabIndex = 0; tabIndex < tabs.length; tabIndex++) {
            var properties = tabs[tabIndex].properties;

            for (var index = 0; index < properties.length; index++) {
                let currentProperty = properties[index];

                if (isPropertyValid(currentProperty.view)) {
                    vm.currentNodeProperties.push(currentProperty);
                }
            }
        }

        umbracoCmsIntegrationsSemrushResource.getDataSources().then(function (response) {
            if (response.Items.length > 0) {
                vm.dataSourceItems = response.Items;
            }
        });

        umbracoCmsIntegrationsSemrushResource.validateToken().then(function (response) {

            if (response.IsExpired) {
                vm.isConnected = false;
            } else {
                if (response.IsValid === false) {
                    umbracoCmsIntegrationsSemrushResource.refreshAccessToken();
                }
            }

        });

        umbracoCmsIntegrationsSemrushResource.getAuthorizationUrl().then(function (response) {
            vm.authorizationUrl = response;
        });

        umbracoCmsIntegrationsSemrushResource.getTokenDetails().then(function (response) {
            vm.isConnected = response.isAccessTokenAvailable;
        });

        // event handlers
        vm.onConnectClick = function () {
            vm.authWindow = window.open(vm.authorizationUrl,
                "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        }

        vm.onPropertyChange = function () {
            vm.searchKeywordsBoxVisible = vm.selectedProperty.length > 0;
            vm.searchQuery = vm.selectedProperty.length > 0
                ? vm.currentNodeProperties.find(obj => {
                    return obj.alias == vm.selectedProperty;
                }).value
                : '';
        }

        vm.onSearchKeywords = function () {
            // validation
            let isConfigurationValid = isSemrushConfigurationValid();
            if (isConfigurationValid === false) {
                vm.showError("Search Keywords", "Please select a data source and method.");
                return;
            }

            searchRelatedPhrases(1);
        }

        // authorization listener
        window.addEventListener("message", function (event) {
            if (event.data.type === "semrush:oauth:success") {

                var codeParam = "?code=";

                vm.authWindow.close();

                var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length);

                umbracoCmsIntegrationsSemrushResource.getAccessToken(code).then(function (response) {
                    if (response !== "error") {
                        vm.isConnected = true;
                        vm.showSuccess("Semrush authentication", "Access Approved");
                    } else {
                        vm.showError("Semrush authentication", "Access Denied");
                    }
                });


            } else if (event.data.type === "semrush:oauth:denied") {
                vm.showError("Semrush authentication", "Access Denied");

                vm.authWindow.close();

                revokeToken();
            }

        }, false);

        function revokeToken() {
            umbracoCmsIntegrationsSemrushResource.revokeToken().then(function () {
                vm.isConnected = false;
            });
        }

        function searchRelatedPhrases(pageNumber) {
            vm.loading = true;
            umbracoCmsIntegrationsSemrushResource.getRelatedPhrases(vm.searchQuery, pageNumber, vm.selectedDataSource, vm.selectedMethod)
                .then(function (response) {

                    vm.loading = false;

                    if (response.isSuccessful === false) {
                        vm.showError("SEMrush API", response.error);
                    } else {
                        vm.pagination = {
                            pageNumber: vm.pagination.pageNumber,
                            totalPages: response.TotalPages
                        };

                        vm.relatedPhrasesList = response;
                    }
                });
        }

        // notifications
        vm.showSuccess = function (headline, message) {
            notificationsService.success(headline, message);
        }

        vm.showWarning = function (headline, message) {
            notificationsService.warning(headline, message);
        }

        vm.showError = function (headline, message) {
            notificationsService.error(headline, message);
        }

        // pagination
        function nextPage(pageNumber) {
            searchRelatedPhrases(pageNumber);
        }

        function prevPage(pageNumber) {
            searchRelatedPhrases(pageNumber);
        }

        function changePage(pageNumber) {
            searchRelatedPhrases(pageNumber);
        }

        function goToPage(pageNumber) {
            searchRelatedPhrases(pageNumber);
        }

        // status
        vm.onViewStatus = function () {
            var options = {
                title: "SEMrush Authorization Details",
                view: "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/statusEditor.html",
                size: "small",
                revoke: function() {
                    vm.isConnected = false;
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            }

            editorService.open(options);
        }

        function isPropertyValid(propertyTypeAlias) {
            return (propertyTypeAlias === umbPropertyTypeAlias.TEXTBOX
                || propertyTypeAlias === umbPropertyTypeAlias.TEXTAREA
                );
        }

        function isSemrushConfigurationValid() {
            return vm.selectedDataSource !== undefined && vm.selectedMethod !== undefined
                && vm.selectedDataSource.length > 0 && vm.selectedMethod.length > 0;
        }
    }

    angular.module('umbSemrushModule')
        .controller('UmbracoCms.Integrations.SemrushController', semrushController);
})();