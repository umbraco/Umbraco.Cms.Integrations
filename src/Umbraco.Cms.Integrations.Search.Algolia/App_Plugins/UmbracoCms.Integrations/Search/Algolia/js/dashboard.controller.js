function dashboardController(notificationsService, overlayService, eventsService, algoliaService, umbracoCmsIntegrationsSearchAlgoliaResource) {
    var vm = this;

    vm.loading = false;

    vm.searchQuery = "";
    vm.selectedSearchIndex = {};
    vm.searchResults = {};

    vm.viewState = "list";

    init();

    vm.addIndex = addIndex;
    vm.saveIndex = saveIndex;
    vm.viewIndex = viewIndex;
    vm.buildIndex = buildIndex;
    vm.searchIndex = searchIndex;
    vm.deleteIndex = deleteIndex;
    vm.search = search;

    function init() {
        /* contentData property:
          [
	        {
		        "contentType": {
			        "alias": "",
			        "name": "",
			        "icon": ""
		        },
		        "properties": [
			        { 
				        "alias": "", 
				        "name": "" 
			        }
		        ]
	        }
          ]
         */
        vm.manageIndex = {
            id: 0,
            name: "",
            selectedContentType: {},
            contentTypesList: [],
            propertiesList: [],
            includeProperties: [
                {
                    "alias": "alias",
                    "header": "Alias"
                },
                {
                    "alias": "group",
                    "header": "Group"
                }
            ],
            contentData: [],
            showProperties: function (contentType) {

                this.selectedContentType = contentType;

                algoliaService.getPropertiesByContentTypeId(contentType.id, (response) => {
                    this.propertiesList = response;

                    var contentTypeData = this.contentData.find(obj => obj.contentType.alias == contentType.alias);
                    if (contentTypeData && contentTypeData.properties.length > 0) {
                        vm.manageIndex.propertiesList = vm.manageIndex.propertiesList.map((obj) => {
                            if (contentTypeData.properties.find(p => p.alias == obj.alias)) {
                                obj.selected = true;
                            }

                            return obj;
                        });
                    }
                });
            },
            removeContentType: function (contentType) {

                const contentTypeIndex = this.contentData.map(obj => obj.contentType.alias).indexOf(contentType.alias);
                this.contentData.splice(contentTypeIndex, 1);

                this.selectedContentType = {};
                this.contentTypesList.forEach(obj => {
                    if (obj.alias == contentType.alias) {
                        obj.selected = false;
                        obj.allowRemove = false;
                    }
                });
                this.propertiesList = [];
            },
            selectProperty: function (property) {

                var contentDataItem = vm.manageIndex.contentData.find(obj => obj.contentType.alias == vm.manageIndex.selectedContentType.alias);

                var selected = !property.selected;
                if (selected) {
                    // mark item selected
                    vm.manageIndex.propertiesList.find(obj => obj.alias == property.alias).selected = true;

                    // check if content type exists in the contentData array
                    if (!contentDataItem) {
                        var contentItem = {
                            contentType: {
                                alias: vm.manageIndex.selectedContentType.alias,
                                name: vm.manageIndex.selectedContentType.name,
                                icon: vm.manageIndex.selectedContentType.icon
                            },
                            properties: []
                        };
                        vm.manageIndex.contentData.push(contentItem);

                        // select content type
                        vm.manageIndex.contentTypesList.forEach(obj => {
                            if (obj.alias == vm.manageIndex.selectedContentType.alias) {
                                obj.selected = true;
                                obj.allowRemove = true;
                            }
                        });
                    }

                    // add property
                    vm.manageIndex.contentData
                        .find(obj => obj.contentType.alias == vm.manageIndex.selectedContentType.alias)
                        .properties.push({
                            alias: property.alias,
                            name: property.name
                        });
                }
                else {
                    // deselect item
                    vm.manageIndex.propertiesList.find(obj => obj.alias == property.alias).selected = false;

                    // remove property item
                    const propertyIndex = vm.manageIndex.contentData
                        .find(obj => obj.contentType.alias == vm.manageIndex.selectedContentType.alias)
                            .properties.map(obj => obj.alias).indexOf(property.alias);
                    vm.manageIndex.contentData
                            .find(obj => obj.contentType.alias == vm.manageIndex.selectedContentType.alias).properties.splice(propertyIndex, 1);

                    // remove content type item with no properties and deselect
                    if (vm.manageIndex.contentData.find(obj => obj.contentType.alias == vm.manageIndex.selectedContentType.alias).properties.length == 0) {
                        vm.manageIndex.contentTypesList.find(obj => obj.alias == vm.manageIndex.selectedContentType.alias).selected = false;
                        vm.manageIndex.contentTypesList.find(obj => obj.alias == vm.manageIndex.selectedContentType.alias).allowRemove = false;

                        const contentTypeIndex = vm.manageIndex.contentData.map(obj => obj.contentType.alias).indexOf(vm.manageIndex.selectedContentType.alias);
                        vm.manageIndex.contentData.splice(contentTypeIndex, 1);
                    }
                }
            },
            reset: function () {
                this.visible = false;
                this.id = 0;
                this.name = "";
                this.selectedContentType = {};
                this.contentTypesList = [];
                this.propertiesList = [];
                this.contentData = [];
            }
        };

        getIndices();
    }

    function getIndices() {
        vm.indices = [];

        umbracoCmsIntegrationsSearchAlgoliaResource.getIndices().then(function (data) {
            vm.indices = data;
        });
    }

    function addIndex() {
        vm.viewState = "manage";
        algoliaService.getContentTypes((response) => vm.manageIndex.contentTypesList = response);
    }

    function saveIndex() {

        if (vm.manageIndex.name.length == 0 || vm.manageIndex.contentData.length == 0) {
            notificationsService.error("Algolia", "Index name and content schema are required.");
            return false;
        }
      
        vm.loading = true;

        umbracoCmsIntegrationsSearchAlgoliaResource
            .saveIndex(vm.manageIndex.id, vm.manageIndex.name, vm.manageIndex.contentData)
            .then(function (response) {
                if (response.success) {
                    vm.manageIndex.reset();
                    algoliaService.getContentTypes((response) => vm.manageIndex.contentTypes = response);
                    notificationsService.success("Algolia", "Index saved.");
                } else {
                    notificationsService.error("Algolia", response.error);
                }

                vm.viewState = "list";

                getIndices();

                vm.loading = false;
            });
    }

    function viewIndex(index) {

        vm.viewState = "manage";

        vm.manageIndex.id = index.id;
        vm.manageIndex.name = index.name;
        vm.manageIndex.contentData = index.contentData;

        algoliaService.getContentTypes((response) => {

            vm.manageIndex.contentTypesList = response;

            for (var i = 0; i < vm.manageIndex.contentData.length; i++) {

                vm.manageIndex.contentTypesList.forEach(obj => {
                    if (obj.alias == vm.manageIndex.contentData[i].contentType.alias) {
                        obj.selected = true;
                        obj.allowRemove = true;
                    }
                });
            }
        });
    }

    function buildIndex(index) {
        const dialogOptions = {
            view: "/App_Plugins/UmbracoCms.Integrations/Search/Algolia/views/index.build.html",
            index: index,
            submit: function (model) {
                vm.loading = true;

                umbracoCmsIntegrationsSearchAlgoliaResource.buildIndex(model.index.id).then(function (response) {
                    if (response.failure)
                        notificationsService.warning("Algolia", "An error has occurred while building the index: " + response.error);
                    else {
                        notificationsService.success("Algolia", "Index built successfully.");
                        vm.loading = false;
                        overlayService.close();
                    }
                });
            },
            close: function () {
                overlayService.close();
            }
        };

        overlayService.open(dialogOptions);
    }

    function searchIndex(index) {
        vm.viewState = "search";
        vm.selectedSearchIndex = index;
    }

    function deleteIndex(index) {
        const dialogOptions = {
            title: "Delete",
            content: "Are you sure you want to delete index <b>" + index.name + "</b>?",
            confirmType: "delete",
            submit: function () {
                umbracoCmsIntegrationsSearchAlgoliaResource.deleteIndex(index.id).then(function (response) {
                    if (response.success) {
                        notificationsService.success("Algolia", "Index deleted.");
                        getIndices();
                    } else
                        notificationsService.error("Algolia", response.error);

                    overlayService.close();
                });
            }
        };

        overlayService.confirm(dialogOptions);
    }

    function search() {
        umbracoCmsIntegrationsSearchAlgoliaResource.search(vm.selectedSearchIndex.id, vm.searchQuery).then(function (response) {
            vm.searchResults = response;
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Search.Algolia.DashboardController", dashboardController);