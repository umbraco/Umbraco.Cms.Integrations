function dashboardController(algoliaService, umbracoCmsIntegrationsSearchAlgoliaResource) {
    var vm = this;

    const CREATE_INDEX_DEFINITION = "Create Index Definition";
    const EDIT_INDEX_DEFINITION = "Edit Index Definition";

    vm.loading = false;
    vm.delete = false;

    vm.searchQuery = "";
    vm.selectedSearchIndex = {};
    vm.searchResults = {};

    vm.viewState = "list";

    init();

    vm.addIndex = addIndex;
    vm.saveIndex = saveIndex;
    vm.viewIndex = viewIndex;
    vm.buildIndexConfirm = buildIndexConfirm;
    vm.searchIndex = searchIndex;
    vm.deleteIndex = deleteIndex;
    vm.search = search;
    vm.back = back;

    vm.showToast = showToast;
    vm.showDeleteDialog = showDeleteDialog;
    vm.showBuildDialog = showBuildDialog;

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
            viewTitle: CREATE_INDEX_DEFINITION,
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
            },
            removeProperty: function (property) {
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
            },
            reset: function () {
                this.visible = false;
                this.id = 0;
                this.viewTitle = CREATE_INDEX_DEFINITION
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
            vm.showToast({
                color: 'danger',
                headline: 'Algolia',
                message: 'Index name and content schema are required.'
            });
            return false;
        }

        vm.loading = true;

        umbracoCmsIntegrationsSearchAlgoliaResource
            .saveIndex(vm.manageIndex.id, vm.manageIndex.name, vm.manageIndex.contentData)
            .then(function (response) {
                if (response.success) {
                    vm.manageIndex.reset();
                    algoliaService.getContentTypes((response) => vm.manageIndex.contentTypes = response);
                    vm.showToast({
                        color: 'positive',
                        headline: 'Algolia',
                        message: 'Index saved.'
                    });
                } else {
                    vm.showToast({
                        color: 'danger',
                        headline: 'Algolia',
                        message: response.error
                    });
                }

                vm.viewState = "list";

                getIndices();

                vm.loading = false;
            });
    }

    function viewIndex(index) {

        vm.viewState = "manage";

        vm.manageIndex.id = index.id;
        vm.manageIndex.viewTitle = EDIT_INDEX_DEFINITION;
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

    function buildIndexConfirm(index) {
        vm.showBuildDialog(index);
    }

    function searchIndex(index) {
        vm.viewState = "search";
        vm.selectedSearchIndex = index;
    }

    function deleteIndex(index) {
        vm.showDeleteDialog(index);
    }

    function search() {
        umbracoCmsIntegrationsSearchAlgoliaResource.search(vm.selectedSearchIndex.id, vm.searchQuery).then(function (response) {
            vm.searchResults = response;
        });
    }

    function back() {
        vm.manageIndex.reset();

        vm.searchQuery = '';
        vm.selectedSearchIndex = {};
        vm.searchResults = {};

        vm.viewState = "list";
    }

    /* Toast Config properties:
     *  color
     *  headline
     *  message
     */
    function showToast(config) {
        const con = document.querySelector('uui-toast-notification-container');

        const toast = document.createElement('uui-toast-notification');
        toast.color = config.color;

        const toastLayout = document.createElement('uui-toast-notification-layout');
        toastLayout.headline = config.headline;
        toast.appendChild(toastLayout);

        const messageEl = document.createElement('span');
        messageEl.innerHTML = config.message;
        toastLayout.appendChild(messageEl);

        if (con) {
            con.appendChild(toast);
        }
    }

    /* delete handlers */
    function showDeleteDialog(index) {
        const dialog = document.getElementById('deleteDialog');
        dialog.style.display = "block";

        const p = document.createElement('p');
        p.innerHTML = "Are you sure you want to delete index <b>" + index.name + "</b>?";

        const dialogLayout = dialog.querySelector('uui-dialog-layout');
        dialogLayout.appendChild(p);

        // event listeners
        var btnCancel = dialog.querySelector('#btnCancel');
        btnCancel.addEventListener('click', closeDeleteDialog);

        var btnDelete = dialog.querySelector('#btnDelete');
        btnDelete.addEventListener('click', function () {
            umbracoCmsIntegrationsSearchAlgoliaResource.deleteIndex(index.id).then(function (response) {
                if (response.success) {
                    vm.showToast({
                        color: 'positive',
                        headline: 'Algolia',
                        message: 'Index deleted'
                    });
                    getIndices();
                } else {
                    vm.showToast({
                        color: 'danger',
                        headline: 'Algolia',
                        message: 'An error has occurred: ' + response.error
                    });
                }

                closeDeleteDialog();
            });
        });
    }

    function closeDeleteDialog() {
        const dialog = document.getElementById('deleteDialog');
        dialog.style.display = "none";

        const dialogLayout = dialog.querySelector('uui-dialog-layout');
        const p = dialogLayout.querySelector('p');
        dialogLayout.removeChild(p);
    }

    /* build index handlers */
    function showBuildDialog(index) {
        const dialog = document.getElementById('buildDialog');
        dialog.style.display = "block";

        // add event listeners
        var btnCancel = dialog.querySelector('#btnCancel');
        btnCancel.addEventListener('click', closeBuildDialog);

        var btnBuild = dialog.querySelector('#btnBuild');
        btnBuild.removeEventListener('click', buildIndex);
        btnBuild.addEventListener('click', buildIndex, false);
        btnBuild.indexId = index.id;
    }

    function closeBuildDialog() {
        const dialog = document.getElementById('buildDialog');
        dialog.style.display = "none";
    }

    function buildIndex(event) {
        vm.loading = true;

        umbracoCmsIntegrationsSearchAlgoliaResource.buildIndex(event.currentTarget.indexId).then(function (response) {
            if (response.failure) {
                vm.showToast({
                    color: 'warning',
                    headline: 'Algolia',
                    message: 'An error has occurred while building the index: ' + response.error
                });
            }
            else {
                vm.showToast({
                    color: 'positive',
                    headline: 'Algolia',
                    message: 'Index built successfully'
                });
            }
        });
        vm.loading = false;

        closeBuildDialog();
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Search.Algolia.DashboardController", dashboardController);