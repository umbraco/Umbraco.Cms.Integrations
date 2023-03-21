function mediaPickerEditorController($scope, notificationsService,
    umbracoCmsIntegrationsDamAprimoService, umbracoCmsIntegrationsDamAprimoResource) {

    var vm = this;

    vm.loading = false;
    vm.data = {};
    vm.searchTerm = "";
    vm.browserIsSupported = umbracoCmsIntegrationsDamAprimoService.browserIsSupported();

    vm.pagination = {
        pageNumber: 1,
        totalPages: 1
    };
    vm.nextPage = goToPage;
    vm.prevPage = goToPage;
    vm.changePage = goToPage;
    vm.goToPage = goToPage;

    umbracoCmsIntegrationsDamAprimoResource.checkApiConfiguration().then(function (response) {
        if (response.success) {
            vm.loading = true;
            query(1);
        }
        else
            notificationsService.error("Aprimo", response.error);
    });

    vm.search = () => {
        query(1);
    }

    vm.clearSearch = () => {
        vm.searchTerm = "";
        query(1);   
    }

    vm.save = (id) => {
        $scope.model.save(id);
    }

    function query(page) {
        vm.loading = true;
        umbracoCmsIntegrationsDamAprimoResource.getRecords(page, vm.searchTerm).then(function (recordsResponse) {
            if (recordsResponse.success) {
                vm.data = recordsResponse.data;

                vm.pagination.pageNumber = vm.data.page;
                vm.pagination.totalPages = Math.ceil(vm.data.totalCount / vm.data.pageSize);
            }
            else {
                notificationsService.error("Aprimo", recordsResponse.error);
            }

            vm.loading = false;
        });
    }

    function goToPage(page) {
        query(page);
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.DAM.Aprimo.MediaPickerEditorController", mediaPickerEditorController);