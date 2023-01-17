function entityPickerEditorController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    vm.loading = false;
    vm.entities = [];
    vm.filteredEntities = [];
    vm.searchTerm = "";

    vm.pagination = {
        pageNumber: 1,
        itemsPerPage: 3,
        totalPages: 1
    };

    query();

    function registerListeners() {
        var paginationCtrl = document.querySelector("uui-pagination");
        paginationCtrl.setAttribute("current", 1);
        paginationCtrl.setAttribute("total", vm.pagination.totalPages);
        paginationCtrl.addEventListener("change", function () {
            vm.filteredEntities = vm.entities
                .slice((paginationCtrl.current - 1) * vm.pagination.itemsPerPage, paginationCtrl.current * vm.pagination.itemsPerPage);
        });

        var inSearch = document.getElementById("inSearch");
        inSearch.addEventListener("change", function () {
            console.log(inSearch.value);
        });
    }

    vm.search = search;
    function search() {
        var inSearch = document.getElementById("inSearch");
        var paginationCtrl = document.querySelector("uui-pagination");

        if (inSearch.length == 0) return false;

        let filteredArr = vm.entities.filter(obj => obj.displayName.includes(inSearch.value));
        vm.filteredEntities = filteredArr
                     .slice((paginationCtrl.current - 1) * vm.pagination.itemsPerPage, paginationCtrl.current * vm.pagination.itemsPerPage);
        vm.pagination.totalPages = Math.ceil(filteredArr.length / vm.pagination.itemsPerPage);

        console.log(vm.pagination.totalPages);

        paginationCtrl.setAttribute("total", vm.pagination.totalPages);
    }

    function query() {

        var entityTypeId = $scope.model.configuration.entityType;
        var fieldTypeIds = $scope.model.configuration.displayFieldTypeIds.join(',');

        vm.loading = true;
        umbracoCmsIntegrationsPimInriverResource.query(entityTypeId, fieldTypeIds).then(function (response) {

            if (response.success) {
                vm.entities = response.data.map(obj => {
                    return {
                        id: obj.entityId,
                        displayName: obj.summary.displayName,
                        description: obj.summary.description,
                        displayFields: obj.fieldValues
                    };
                });

                vm.pagination.totalPages = Math.ceil(vm.entities.length / vm.pagination.itemsPerPage);

                vm.filteredEntities = vm.entities.slice(0, vm.pagination.itemsPerPage);

                registerListeners();
            } else
                notificationsService.error("Inriver", response.error);

            vm.loading = false;
        });
    }

    vm.save = function (entityId) {
        $scope.model.save(entityId);
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerEditorController", entityPickerEditorController);