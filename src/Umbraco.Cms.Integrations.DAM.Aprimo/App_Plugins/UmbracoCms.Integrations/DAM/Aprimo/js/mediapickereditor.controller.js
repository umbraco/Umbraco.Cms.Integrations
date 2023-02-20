function mediaPickerEditorController($scope, notificationsService,
    umbracoCmsIntegrationsDamAprimoService, umbracoCmsIntegrationsDamAprimoResource) {

    var vm = this;

    vm.loading = false;
    vm.data = {};
    vm.browserIsSupported = umbracoCmsIntegrationsDamAprimoService.browserIsSupported();

    vm.pagination = {
        pageNumber: 1,
        itemsPerPage: 10,
        totalPages: 1
    };
    const paginationCtrl = document.querySelector("uui-pagination");

    umbracoCmsIntegrationsDamAprimoResource.checkApiConfiguration().then(function (response) {
        if (response.success) {
            vm.loading = true;
            query(1);
        }
        else
            notificationsService.error("Aprimo", response.error);
    });

    vm.openContentSelector = function () {
        umbracoCmsIntegrationsDamAprimoResource.getContentSelectorUrl().then(function (response) {
            if (response.length > 0) {
                window.open(response,
                    "Aprimo Content Selector",
                    "width=900,height=700,modal=yes,alwaysRaised=yes");

                window.addEventListener("message", function (event) {
                    if (event.data.result !== 'cancel' && event.data.selection !== undefined) {
                        $scope.model.save(event.data.selection[0].id);
                    }
                }, false);
            }
            else
                notificationsService.warning("Aprimo", "Could not retrieve content selector URL.");
        });
    }

    vm.save = function (id) {
        $scope.model.save(id);
    }

    function query(page) {
        umbracoCmsIntegrationsDamAprimoResource.getRecords(page).then(function (recordsResponse) {
            if (recordsResponse.success) {
                vm.data = recordsResponse.data;

                vm.pagination.pageNumber = page;
                vm.pagination.totalPages = Math.ceil(vm.data.totalCount / vm.data.pageSize);
                paginationCtrl.total = vm.pagination.totalPages;

                registerListeners();

                vm.loading = false;
            }
            else {
                vm.loading = false;

                notificationsService.error("Aprimo", recordsResponse.error);
            }
        });
    }

    function registerListeners() {
        paginationCtrl.setAttribute("current", 1);
        paginationCtrl.setAttribute("total", vm.pagination.totalPages);
        paginationCtrl.addEventListener("change", function () {
            query(paginationCtrl.current);
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.MediaPickerEditorController", mediaPickerEditorController);