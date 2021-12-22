(() => {

    function semrushToolsController($scope, $window, editorState, notificationsService, editorService, contentResource, umbracoCmsIntegrationsSemrushResources) {

        var vm = this;

        vm.isConnected = false;
        vm.loading = false;

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

        vm.pagination = {
            pageNumber: 1,
            totalPages: 1
        };
        vm.nextPage = nextPage;
        vm.prevPage = prevPage;
        vm.changePage = changePage;
        vm.goToPage = goToPage;

        umbracoCmsIntegrationsSemrushResources.importDataSources().then(function (response) {

            if (response.Items.length > 0) {
                vm.dataSourceItems = response.Items;
            }

        });

        umbracoCmsIntegrationsSemrushResources.validateToken().then(function (response) {

            if (response.IsExpired) {
                vm.isConnected = false;
            } else {
                if (response.IsValid === false) {
                    umbracoCmsIntegrationsSemrushResources.refreshAccessToken().then(function (r) {
                        console.log(r);
                    });
                }
            }

        });

        umbracoCmsIntegrationsSemrushResources.getAuthorizationUrl().then(function (response) {
            vm.authorizationUrl = response;
        });

        umbracoCmsIntegrationsSemrushResources.getTokenDetails().then(function (response) {
            vm.isConnected = response.IsAccessTokenAvailable;
        });

        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

        contentResource.getById(vm.CurrentNodeId).then(function (node) {
            var properties = node.variants[0].tabs[0].properties;

            vm.CurrentNodeProperties = properties;
        });

        // event handlers
        vm.OnConnectClick = function () {
            vm.authWindow = window.open(vm.authorizationUrl,
                "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        }

        vm.OnPropertyChange = function () {

            vm.SelectedPropertyDetails = vm.CurrentNodeProperties.find(obj => {
                return obj.alias == vm.SelectedProperty;
            });

        }

        vm.OnGetRelatedPhrases = function () {
            searchRelatedPhrases(1);
        }

        // authorization listener
        window.addEventListener("message", function (event) {
            if (event.data.type === "semrush:oauth:success") {

                var codeParam = "?code=";

                vm.showSuccess("SEMrush API", "Access Approved");

                vm.authWindow.close();

                var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length);

                umbracoCmsIntegrationsSemrushResources.getAccessToken(code).then(function (response) {
                    console.log(response);

                    if (response !== "error") vm.isConnected = true;
                });


            } else if (event.data.type === "semrush:oauth:denied") {
                vm.showError("SEMrush API", "Access Denied");

                vm.authWindow.close();

                revokeToken();
            }

        }, false);

        function revokeToken() {
            umbracoCmsIntegrationsSemrushResources.revokeToken().then(function () {
                vm.isConnected = false;
            });
        }

        function searchRelatedPhrases(pageNumber, dataSource, method) {
            vm.loading = true;
            umbracoCmsIntegrationsSemrushResources.getRelatedPhrases(vm.SelectedPropertyDetails.value, pageNumber, vm.SelectedDataSource, vm.SelectedMethod)
                .then(function (response) {

                    vm.loading = false;

                    console.log(response);

                    vm.pagination = {
                        pageNumber: vm.pagination.pageNumber,
                        totalPages: response.TotalPages
                    };

                    vm.relatedPhrasesList = response;
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
        vm.OnViewStatus = function () {
            var options = {
                title: "SEMrush Authorization Details",
                view: "/App_Plugins/UmbracoCms.Integrations/SEO/SemrushTools/statusEditor.html",
                size: "small",
                close: function () {

                    vm.isConnected = false;

                    editorService.close();
                }
            }

            editorService.open(options);
        }
    }

    angular.module('umbraco')
        .controller('UmbracoCms.Integrations.SemrushToolsController', semrushToolsController);
})();