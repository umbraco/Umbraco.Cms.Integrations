function entityPickerEditorController($scope, editorService, notificationsService, umbracoCmsIntegrationsPimInriverResource) {

    var vm = this;

    vm.loading = false;
    vm.entities = [];
    vm.filteredEntities = [];
    vm.searchTerm = "";

    const inSearch = document.getElementById("inSearch");
    const paginationCtrl = document.querySelector("uui-pagination");

    vm.pagination = {
        pageNumber: 1,
        itemsPerPage: 5,
        totalPages: 1
    };

    query();

    vm.search = () => {
        if (inSearch.length == 0) return false;

        let filteredArr = vm.entities.filter(obj => obj.summary.displayName.includes(inSearch.value));
        vm.filteredEntities = filteredArr
            .slice((paginationCtrl.current - 1) * vm.pagination.itemsPerPage, paginationCtrl.current * vm.pagination.itemsPerPage);

        vm.pagination.totalPages = Math.ceil(filteredArr.length / vm.pagination.itemsPerPage);

        paginationCtrl.total = vm.pagination.totalPages;

        /**
         * uui-pagination total property is not updating the pagination UI in version of the library lower than 11.0
         * */
        const umbracoVersionSplit = window.Umbraco.Sys.ServerVariables.application.version.split('.');
        if (Number(umbracoVersionSplit[0]) === 10) {
            let visiblePagesArr = [];
            for (var i = 0; i < vm.pagination.totalPages; i++) {
                visiblePagesArr.push(i + 1);
            }

            paginationCtrl._visiblePages = visiblePagesArr;
            paginationCtrl.requestUpdate();
        }
    }

    function query() {

        var entityTypeId = $scope.model.configuration.entityType;
        var fieldTypes = $scope.model.configuration.fieldTypes;

        vm.loading = true;
        umbracoCmsIntegrationsPimInriverResource.query(entityTypeId, fieldTypes).then(function (response) {
            if (response.success) {
                vm.entities = response.data.map(obj => {
                    return {
                        id: obj.entityId,
                        summary: obj.summary,
                        fields: obj.fieldValues.map(fieldObj => {
                            return {
                                fieldType: fieldTypes.find(fieldTypeObj => fieldTypeObj.fieldTypeId === fieldObj.fieldTypeId).fieldTypeDisplayName,
                                value: fieldObj.display
                            }
                        })
                    }
                });

                vm.pagination.totalPages = Math.ceil(vm.entities.length / vm.pagination.itemsPerPage);

                vm.filteredEntities = vm.entities.slice(0, vm.pagination.itemsPerPage);

                registerListeners();

            } else
                notificationsService.error("Inriver", response.error);

            vm.loading = false;
        });
    }

    function registerListeners() {
        paginationCtrl.setAttribute("current", 1);
        paginationCtrl.setAttribute("total", vm.pagination.totalPages);
        paginationCtrl.addEventListener("change", function () {
            vm.filteredEntities = vm.entities
                .slice((paginationCtrl.current - 1) * vm.pagination.itemsPerPage, paginationCtrl.current * vm.pagination.itemsPerPage);
        });
    }

    vm.save = function (entityId) {
        $scope.model.save(entityId);
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.PIM.Inriver.EntityPickerEditorController", entityPickerEditorController);