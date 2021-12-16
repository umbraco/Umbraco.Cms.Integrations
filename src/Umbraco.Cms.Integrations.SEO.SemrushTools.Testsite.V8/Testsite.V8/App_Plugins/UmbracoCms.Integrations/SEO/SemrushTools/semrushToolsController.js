(() => {

    function semrushToolsController($scope, $window, editorState, contentResource, notificationsService, umbracoCmsIntegrationsSemrushResources) {

        var vm = this;

        vm.loading = false;

        vm.items = [
            {
                "icon": "icon-document",
                "name": "My node 1",
                "published": true,
                "description": "A short description of my node",
                "author": "Author 1"
            },
            {
                "icon": "icon-document",
                "name": "My node 2",
                "published": true,
                "description": "A short description of my node",
                "author": "Author 2"
            }
        ];

        vm.isConnected = false;

        umbracoCmsIntegrationsSemrushResources.getTokenDetails().then(function (response) {
            console.log(response);
            vm.isConnected = response.IsAccessTokenAvailable;
        });

        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

        contentResource.getById(vm.CurrentNodeId).then(function (node) {
            var properties = node.variants[0].tabs[0].properties;

            vm.CurrentNodeProperties = properties;
        });

        vm.OnConnectClick = function() {
            vm.authWindow = window.open("https://oauth.semrush.com/oauth2/authorize?ref=0053752252&client_id=umbraco&redirect_uri=%2Foauth2%2Fumbraco%2Fsuccess&response_type=code&scope=user.id,domains.info,url.info,positiontracking.info",
                "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        }

        vm.OnPropertyChange = function () {

            vm.SelectedPropertyDetails = vm.CurrentNodeProperties.find(obj => {
                return obj.alias == vm.SelectedProperty;
            });

        }

        vm.OnGetRelatedPhrases = function () {
            vm.loading = true;
            umbracoCmsIntegrationsSemrushResources.getRelatedPhrases(vm.SelectedPropertyDetails.value).then(function (response) {
                vm.loading = false;

                console.log(response);

                vm.relatedPhrasesList = response;
            });
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
                });


            } else if (event.data.type === "semrush:oauth:denied") {
                vm.showError("SEMrush API", "Access Denied");

                vm.authWindow.close();
            }

        }, false);

        // notifications
        vm.showSuccess = function (headline, message) {
            notificationsService.success(headline, message);
        }

        vm.showWarning = function(headline, message) {
            notificationsService.warning(headline, message);
        }

        vm.showError = function (headline, message) {
            notificationsService.error(headline, message);
        }
    }

    angular.module('umbraco')
        .controller('UmbracoCms.Integrations.SemrushToolsController', semrushToolsController);
})();