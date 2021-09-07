
/**
 * The controller that is used for picking content from CommerceTools,
 * since this supports both categories, products and product variants, there is code to deal with all 3 of those types
 * @param {any} $scope
 * @param {any} $q
 * @param {any} commerceToolsResource
 * @param {any} editorState
 * @param {any} localizationService
 * @param {any} editorService
 * @param {any} overlayService
 */
function commerceToolsPickerController($scope, $q, $routeParams, commerceToolsResource, editorState, localizationService, editorService, overlayService, umbRequestHelper) {

    var vm = {
        labels: {
            general_recycleBin: "",
            general_add: ""
        }
    };

    var unsubscribe;

    function subscribe() {
        unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
            var currIds = _.map($scope.renderModel, function (i) {
                return i.id;
            });
            $scope.model.value = trim(currIds.join(), ",");
        });
    }

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    /** Performs validation based on the renderModel data */
    function validate() {
        if ($scope.commerceToolsPickerForm) {
            //Validate!
            if ($scope.model.config && $scope.model.config.minNumber && parseInt($scope.model.config.minNumber) > $scope.renderModel.length) {
                $scope.commerceToolsPickerForm.minCount.$setValidity("minCount", false);
            }
            else {
                $scope.commerceToolsPickerForm.minCount.$setValidity("minCount", true);
            }

            if ($scope.model.config && $scope.model.config.maxNumber && parseInt($scope.model.config.maxNumber) < $scope.renderModel.length) {
                $scope.commerceToolsPickerForm.maxCount.$setValidity("maxCount", false);
            }
            else {
                $scope.commerceToolsPickerForm.maxCount.$setValidity("maxCount", true);
            }
        }
    }

    function startWatch() {

        //due to the way angular-sortable works, it needs to update a model, we don't want it to update renderModel since renderModel
        //is updated based on changes to model.value so if we bound angular-sortable to that and put a watch on it we'd end up in a
        //infinite loop. Instead we have a custom array model for angular-sortable and we'll watch that which we'll use to sync the model.value
        //which in turn will sync the renderModel.
        $scope.$watchCollection("sortableModel", function (newVal, oldVal) {
            $scope.model.value = newVal.join();
        });

        //if the underlying model changes, update the view model, this ensures that the view is always consistent with the underlying
        //model if it changes (i.e. based on server updates, or if used in split view, etc...)
        $scope.$watch("model.value", function (newVal, oldVal) {
            if (newVal !== oldVal) {
                syncRenderModel(true);
            }
        });
    }

    $scope.renderModel = [];
    $scope.sortableModel = [];

    $scope.labels = vm.labels;

    $scope.dialogEditor = editorState && editorState.current && editorState.current.isDialogEditor === true;

    //the default pre-values
    var defaultConfig = {
        validationLimit: {
            min: null,
            max: null
        },
        entityType: "Product",
        pageSize: 20,
        orderBy: "Name",
        orderDirection: "Ascending"
    };

    // sortable options
    $scope.sortableOptions = {
        axis: "y",
        containment: "parent",
        distance: 10,
        opacity: 0.7,
        tolerance: "pointer",
        scroll: true,
        zIndex: 6000,
        update: function (e, ui) {
            setDirty();
        }
    };

    var removeAllEntriesAction = {
        labelKey: 'clipboard_labelForRemoveAllEntries',
        labelTokens: [],
        icon: 'trash',
        method: removeAllEntries,
        isDisabled: true
    };

    if ($scope.model.config) {
        //merge the server config on top of the default config, then set the server config to use the result
        $scope.model.config = Utilities.extend(defaultConfig, $scope.model.config);

        // if the property is mandatory, set the minCount config to 1 (unless of course it is set to something already),
        // that way the minCount/maxCount validation handles the mandatory as well
        if ($scope.model.validation && $scope.model.validation.mandatory && !$scope.model.config.minNumber) {
            $scope.model.config.minNumber = 1;
        }

        if ($scope.model.config.multiPicker === true && $scope.umbProperty) {
            var propertyActions = [
                removeAllEntriesAction
            ];

            $scope.umbProperty.setPropertyActions(propertyActions);
        }

        $scope.model.config.multiPicker = !$scope.model.config.validationLimit.max || parseInt($scope.model.config.validationLimit.max) > 1;
    }


    var entityType = $scope.model.config.entityType;

    $scope.currentPicker = angular.copy($scope.model.config);
    //dialog
    $scope.openCurrentPicker = function () {

        $scope.currentPicker.submit = function (model) {
            $scope.model.value = $scope.model.value.split(',').concat(model.split(',')).filter(function (value, index, array) { return value && array.indexOf(value) === index }).join(',');
            setDirty();
            editorService.close();
        }

        $scope.currentPicker.close = function () {
            editorService.close();
        }

        $scope.currentPicker.view = umbRequestHelper.convertVirtualToAbsolutePath("~/App_Plugins/UmbracoCms.Integrations/Commerce/CommerceTools/Overlays/CommerceToolsPickerOverlay.html"),

        editorService.open($scope.currentPicker);

    };

    updateModelValue = function (newValue) {

    }

    $scope.remove = function (index) {
        var currIds = $scope.model.value ? $scope.model.value.split(',') : [];
        if (currIds.length > 0) {
            currIds.splice(index, 1);
            setDirty();
            $scope.model.value = currIds.join();
        }

        removeAllEntriesAction.isDisabled = currIds.length === 0;
    };

    $scope.clear = function () {
        $scope.model.value = null;
        removeAllEntriesAction.isDisabled = true;
    };

    //when the scope is destroyed we need to unsubscribe
    $scope.$on('$destroy', function () {
        if (unsubscribe) {
            unsubscribe();
        }
    });

    function setDirty() {
        if ($scope.commerceToolsPickerForm && $scope.commerceToolsPickerForm.modelValue) {
            $scope.commerceToolsPickerForm.modelValue.$setDirty();
        }
    }

    /** Syncs the renderModel based on the actual model.value and returns a promise */
    function syncRenderModel(doValidation) {

        var valueIds = $scope.model.value ? $scope.model.value.split(',') : [];

        //sync the sortable model
        $scope.sortableModel = valueIds;

        removeAllEntriesAction.isDisabled = valueIds.length === 0;

        //load current data if anything selected
        if (valueIds.length > 0) {

            //need to determine which items we already have loaded
            var renderModelIds = _.map($scope.renderModel, function (d) {
                return d.id.toString();
            });

            //get the ids that no longer exist
            var toRemove = _.difference(renderModelIds, valueIds);


            //remove the ones that no longer exist
            for (var j = 0; j < toRemove.length; j++) {
                var index = renderModelIds.indexOf(toRemove[j]);
                $scope.renderModel.splice(index, 1);
            }

            //get the ids that we need to lookup entities for
            var missingIds = _.difference(valueIds, renderModelIds);

            if (missingIds.length > 0) {
                return commerceToolsResource.getByIds(missingIds, entityType, $routeParams.mculture ? $routeParams.mculture : null).then(function (data) {

                    _.each(valueIds,
                        function (id, i) {
                            var entity = _.find(data, function (d) {
                                return d.id == id;
                            });

                            if (entity) {
                                addSelectedItem(entity);
                            }

                        });

                    if (doValidation) {
                        validate();
                    }

                    setSortingState($scope.renderModel);
                    return $q.when(true);
                });
            }
            else {
                //if there's nothing missing, make sure it's sorted correctly

                var current = $scope.renderModel;
                $scope.renderModel = [];
                for (var k = 0; k < valueIds.length; k++) {
                    var id = valueIds[k];
                    var found = _.find(current, function (d) {
                        return d.id == id;
                    });
                    if (found) {
                        $scope.renderModel.push(found);
                    }
                }

                if (doValidation) {
                    validate();
                }

                setSortingState($scope.renderModel);
                return $q.when(true);
            }
        }
        else {
            $scope.renderModel = [];
            if (doValidation) {
                validate();
            }
            setSortingState($scope.renderModel);
            return $q.when(true);
        }

    }

    function addSelectedItem(item) {

        // set default icon
        if (!item.icon) {
            switch (entityType) {
                case "Category":
                    item.icon = "icon-network-alt";
                    break;
                case "Product":
                    item.icon = "icon-box";
                    break;
            }
        }

        $scope.renderModel.push({
            "name": item.name,
            "id": item.id,
            "icon": item.icon,
            "description": item.key,
        });
    }

    function setSortingState(items) {
        // disable sorting if the list only consist of one item
        if (items.length > 1) {
            $scope.sortableOptions.disabled = false;
        } else {
            $scope.sortableOptions.disabled = true;
        }
    }

    function removeAllEntries() {
        localizationService.localizeMany(["content_nestedContentDeleteAllItems", "general_delete"]).then(function (data) {
            overlayService.confirmDelete({
                title: data[1],
                content: data[0],
                close: function () {
                    overlayService.close();
                },
                submit: function () {
                    $scope.clear();
                    overlayService.close();
                }
            });
        });
    }

    function init() {

        localizationService.localizeMany(["general_recycleBin", "general_add", "commerceTools_select" + entityType + ($scope.model.config.multiPicker ? "Multi" : "")])
            .then(function (data) {
                vm.labels.general_recycleBin = data[0];
                vm.labels.general_add = data[1];
                $scope.currentPicker.title = data[2];

                syncRenderModel(false).then(function () {
                    //everything is loaded, start the watch on the model
                    startWatch();
                    subscribe();
                    validate();
                });
            });
    }

    init();

}

angular.module('umbraco').controller("Umbraco.Cms.Integrations.Commerce.CommerceTools.PropertyEditors.CommerceToolsPickerController", commerceToolsPickerController);