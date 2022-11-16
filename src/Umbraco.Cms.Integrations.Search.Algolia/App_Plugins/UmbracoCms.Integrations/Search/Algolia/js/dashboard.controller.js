function dashboardController(notificationsService, algoliaService, umbracoCmsIntegrationsSearchAlgoliaResource) {
    var vm = this;

    vm.searchQuery = "";
    vm.searchIndex = {};
    vm.searchResults = {};

    vm.viewState = "list";

    init();

    vm.addIndex = addIndex;
    vm.saveIndex = saveIndex;
    vm.viewIndex = viewIndex;
    vm.deleteIndex = deleteIndex;
    vm.search = search;

    function init() {

        // contentData property:
        //   array of objects:
        //     contentType -> string value
        //     properties -> string array
        vm.manageIndex = {
            id: 0,
            name: "",
            selectedContentType: "",
            contentTypes: [],
            properties: [],
            contentData: [],
            selectContentType: function (contentType) {

                this.properties = [];

                var checked = !contentType.checked;

                if (checked) {

                    this.selectedContentType = contentType.alias;
                    this.contentTypes.find(p => p.alias == contentType.alias).checked = true;

                    var contentItem = {
                        contentType: contentType.alias,
                        properties: []
                    };

                    this.contentData.push(contentItem);
                }
                else {
                    this.contentTypes.find(p => p.alias == contentType.alias).checked = false;

                    const contentTypeIndex = this.contentData.findIndex((obj) => obj.contentType === contentType.alias);

                    if (contentTypeIndex > -1) this.contentData.splice(contentTypeIndex, 1);

                    this.selectedContentType = "";

                    this.properties = [];
                }
            },
            showProperties: function (contentType) {

                algoliaService.getPropertiesByContentTypeId(contentType.id, (response) => {
                    vm.manageIndex.properties = response;

                    var contentTypeData = this.contentData.find(p => p.contentType == contentType.alias);
                    if (contentTypeData && contentTypeData.properties.length > 0) {
                        vm.manageIndex.properties = vm.manageIndex.properties.map((obj) => {
                            if (contentTypeData.properties.find(p => p == obj.alias)) {
                                obj.checked = true;
                            }

                            return obj;
                        });
                    }
                });
            },
            selectProperty: function (property) {
                var checked = !property.checked;

                if (this.contentData.length == 0 || this.contentData.find(p => p.contentType === this.selectedContentType) === undefined) {
                    notificationsService.warning("Please select the property matching content type.");
                    return false;
                }

                if (checked) {
                    this.properties.find(p => p.alias == property.alias).checked = true;
                    this.contentData.find(p => p.contentType === this.selectedContentType).properties.push(property.alias);
                }
                else {
                    const propertyIndex = this.contentData.find(p => p.contentType === this.selectedContentType).properties.indexOf(property.alias);
                    if (propertyIndex > -1) this.contentData.find(p => p.contentType === this.selectedContentType).properties.splice(propertyIndex, 1);
                }
            },
            reset: function () {
                this.visible = false;
                this.name = "";
                this.selectedContentType = "";
                this.contentTypes = [];
                this.properties = [];
                this.contentData = [];
            }
        };

        algoliaService.getContentTypes((response) => vm.manageIndex.contentTypes = response);

        getIndices();
    }

    function addIndex() {
        vm.viewState = "manage";
    }

    function saveIndex() {

        if (vm.manageIndex.name.length == 0 || vm.manageIndex.contentData.length == 0) {
            notificationsService.error("Index name and content schema are required");
            return false;
        }
        umbracoCmsIntegrationsSearchAlgoliaResource
            .saveIndex(vm.manageIndex.name, vm.manageIndex.contentData)
            .then(function (response) {
                if (response.length == 0) {
                    vm.manageIndex.reset();
                    algoliaService.getContentTypes((response) => vm.manageIndex.contentTypes = response);
                } else {
                    notificationsService.error(response);
                }

                vm.viewState = "list";

                getIndices();
            });
    }

    function viewIndex(index) {

        vm.viewState = "manage";

        vm.manageIndex.id = index.id;
        vm.manageIndex.name = index.name;
        vm.manageIndex.contentData = index.contentData;

        algoliaService.getContentTypes((response) => {

            vm.manageIndex.contentTypes = response;
            
            for (var i = 0; i < vm.manageIndex.contentData.length; i++) {
                vm.manageIndex.contentTypes.find(p => p.alias === vm.manageIndex.contentData[i].contentType).checked = true;
            }
        });
    }

    function deleteIndex(index) {
        umbracoCmsIntegrationsSearchAlgoliaResource.deleteIndex(index.id).then(function (response) {
            getIndices();
        });
    }

    function search() {
        umbracoCmsIntegrationsSearchAlgoliaResource.search(vm.searchIndex.id, vm.searchQuery).then(function (response) {
            vm.searchResults = response;
        });
    }

    function getIndices() {
        vm.indices = [];

        umbracoCmsIntegrationsSearchAlgoliaResource.getIndices().then(function (data) {
            vm.indices = data;
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Search.Algolia.DashboardController", dashboardController);