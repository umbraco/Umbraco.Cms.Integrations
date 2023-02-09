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

    function semrushController($scope, $window, editorState,
        notificationsService, editorService, userService,
        umbracoCmsIntegrationsSemrushResource, umbPropertyTypeAlias) {

        var vm = this;

        userService.getCurrentUser().then(function (user) {
            var isPartOfAdminUserGroup = user.userGroups.find(x => x === "admin");

            vm.isAdmin = isPartOfAdminUserGroup !== undefined;
        });

        vm.searchKeywordsBoxVisible = false;
        vm.isConnected = false;
        vm.loading = false;

        vm.searchQuery = '';

        vm.selectedDataSource = '';
        vm.selectedMethod = '';

        vm.dataSourceItems = [];
        vm.columnItems = [];

        vm.supportedMethods = [
            {
                "key": "phrase_fullsearch",
                "value": "phrase_fullsearch",
                "description": "List of broad matches and alternate search queries, including particular keywords or keyword expressions."
            },
            {
                "key": "phrase_kdi",
                "value": "phrase_kdi",
                "description": "Keyword difficulty - an index that helps to estimate how difficult it would be to seize competitor's positions " +
                    "in organic search within Google's top 100 with an indicated search term."
            },
            {
                "key": "phrase_organic",
                "value": "phrase_organic",
                "description": "List of domains that are ranking in Google's top 100 organic search results with a requested keyword."
            },
            {
                "key": "phrase_related",
                "value": "phrase_related",
                "description": "Extended list of related keywords, synonyms and variations relevant to a queried term in a chosen database."
            },
            {
                "key": "phrase_these",
                "value": "phrase_these",
                "description": "Summary of up to 100 keywords, including volume, CPC, competition and the number of results in a chosen regional database."
            },
            {
                "key": "phrase_this",
                "value": "phrase_this",
                "description": "Summary of a keyword, including volume, CPC, competition and the number of results in a chosen regional database."
            }
        ];
        vm.searchKeywordsList = {};

        vm.contentProperties = [];

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

            let tabItem = {
                label: tabs[tabIndex].label,
                properties: []
            };

            for (var index = 0; index < properties.length; index++) {
                let currentProperty = properties[index];

                if (isPropertyValid(currentProperty.view)) {
                    tabItem.properties.push(currentProperty);
                }
            }

            if (tabItem.properties.length > 0)
                vm.contentProperties.push(tabItem);
        }

        umbracoCmsIntegrationsSemrushResource.getDataSources().then(function (response) {
            if (response.Items.length > 0) {
                vm.dataSourceItems = response.Items;
            }
        });

        umbracoCmsIntegrationsSemrushResource.getColumns().then(function (response) {
            if (response.length > 0) {
                vm.columnItems = response;
            }
        });

        validateToken();

        umbracoCmsIntegrationsSemrushResource.getAuthorizationUrl().then(function (response) {
            vm.authorizationUrl = response;
        });

        umbracoCmsIntegrationsSemrushResource.getTokenDetails().then(function (response) {
            vm.isConnected = response.isAccessTokenAvailable;
        });

        // event handlers
        vm.onConnectClick = function () {

            window.addEventListener("message", getAccessToken, false);
            vm.authWindow = window.open(vm.authorizationUrl,
                "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        }

        vm.onPropertyChange = function () {
            vm.searchKeywordsBoxVisible = vm.selectedProperty.length > 0;

            if (vm.selectedProperty.length > 0) {
                for (var i = 0; i < vm.contentProperties.length; i++) {
                    for (var j = 0; j < vm.contentProperties[i].properties.length; j++) {
                        if (vm.contentProperties[i].properties[j].alias === vm.selectedProperty) {
                            vm.searchQuery = vm.contentProperties[i].properties[j].value;
                            break;
                        }
                    }
                }
            } else {
                vm.searchQuery = '';
            }
        }

        vm.onSearchKeywords = function () {
            // validation
            let isConfigurationValid = isSemrushConfigurationValid();
            if (isConfigurationValid === false) {
                vm.showError("Search Keywords", "Please select a data source and method.");
                return;
            }

            searchKeywords(1);
        }

        function getAccessToken(event) {
            if (event.data.type === "semrush:oauth:success") {

                var codeParam = "?code=";

                if (vm.authWindow) vm.authWindow.close();

                var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length);

                umbracoCmsIntegrationsSemrushResource.getAccessToken(code).then(function (response) {
                    if (response !== "error") {
                        vm.isConnected = true;
                        vm.showSuccess("Semrush authentication", "Access Approved");
                        validateToken();
                    } else {
                        vm.showError("Semrush authentication", "Access Denied");
                    }
                });

            } else if (event.data.type === "semrush:oauth:denied") {
                vm.showError("Semrush authentication", "Access Denied");

                vm.authWindow.close();

                revokeToken();
            }
        }

        function validateToken() {
            umbracoCmsIntegrationsSemrushResource.validateToken().then(function (response) {
                vm.isFreeAccount = response.isFreeAccount;
                if (response.isExpired) {
                    vm.isConnected = false;
                } else {
                    if (response.isValid === false) {
                        umbracoCmsIntegrationsSemrushResource.refreshAccessToken();
                    }
                }
            });
        }

        function revokeToken() {
            umbracoCmsIntegrationsSemrushResource.revokeToken().then(function () {
                vm.isConnected = false;
                vm.isFreeAccount = null;
                vm.searchKeywordsList = {};

                window.removeEventListener("message", getAccessToken);
            });
        }

        function searchKeywords(pageNumber) {
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

                        vm.searchKeywordsList = response;
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
            searchKeywords(pageNumber);
        }

        function prevPage(pageNumber) {
            searchKeywords(pageNumber);
        }

        function changePage(pageNumber) {
            searchKeywords(pageNumber);
        }

        function goToPage(pageNumber) {
            searchKeywords(pageNumber);
        }

        // status
        vm.onViewStatus = function () {
            var options = {
                title: "SEMrush Authorization Details",
                isFreeAccount: vm.isFreeAccount,
                view: "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/statusEditor.html",
                size: "small",
                revoke: function () {
                    revokeToken();

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

        // tooltips

        // data sources select
        vm.dataSourcesTooltip = {
            show: false,
            event: null
        };

        vm.dataSourcesMouseOver = function ($event) {
            if (vm.selectedDataSource.length > 0) {
                vm.dataSourcesTooltip = {
                    show: true,
                    event: $event
                };
            }
        };

        vm.dataSourcesMouseLeave = function () {
            vm.dataSourcesTooltip = {
                show: false,
                event: null
            };
        }

        // supported methods select
        vm.supportedMethodsTooltip = {
            show: false,
            event: null
        };

        vm.supportedMethodsMouseOver = function ($event) {
            if (vm.selectedMethod.length > 0) {
                vm.supportedMethodsTooltip = {
                    show: true,
                    event: $event
                };
            }
        };

        vm.supportedMethodsMouseLeave = function () {
            vm.supportedMethodsTooltip = {
                show: false,
                event: null
            };
        }

        // table columns
        vm.columnsTooltip = {
            show: false,
            event: null,
            content: ''
        };

        vm.columnsTooltipMouseOver = function ($event, columnName) {

            let columnItem = vm.columnItems.find(x => x.name === columnName);
            if (columnItem !== undefined && columnItem !== null) {
                console.log(columnItem.description);
                vm.columnsTooltip = {
                    show: true,
                    event: $event,
                    content: columnItem.description
                };
            }
        };

        vm.columnsTooltipMouseLeave = function () {
            vm.columnsTooltip = {
                show: false,
                event: null,
                content: ''
            };
        }
    }

    angular.module('umbSemrushModule')
        .controller('UmbracoCms.Integrations.SemrushController', semrushController);
})();