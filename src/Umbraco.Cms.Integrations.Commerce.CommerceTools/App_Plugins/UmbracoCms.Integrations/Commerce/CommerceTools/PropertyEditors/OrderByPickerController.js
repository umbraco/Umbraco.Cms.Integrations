function commerceToolsOrderByPickerController($scope, eventsService) {

    var vm = this;
    vm.entityType = "";

    var availableOptions = [
        {
            key: "Description",
            label: "Description",
            visibleFor: ["Category"],
        },
        {
            key: "Id",
            label: "Id",
            visibleFor: ["Category", "Product"],
        },
        {
            key: "ImageUrl",
            label: "Image",
            visibleFor: ["Product"],
        },
        {
            key: "Key",
            label: "Key",
            visibleFor: ["Product"],
        },
        {
            key: "Name",
            label: "Name",
            visibleFor: ["Category", "Product"],
        },
        {
            key: "SKU",
            label: "SKU",
            visibleFor: ["Product"],
        }
    ]

    var setAvailableOptions = function () {
        vm.entityType = "";

        // find the scope containing all other prevalues, by crawling up the tree
        // model is for v8
        // vm.dataType is for v9
        var scope = $scope.$parent;
        while (scope && !scope.vm?.dataType?.preValues && !scope.model?.preValues) {
            scope = scope.$parent;
        }
        var preValues = (scope?.vm?.dataType ?? scope?.model)?.preValues;


        // if the scope has been found, find the entityType prevalue and read the value
        // v.alias for finding the preValue in v8
        // v.key for finding the preValue in v9
        if (preValues) {
            var entityTypePreValue = preValues.filter(v => v.key === 'entityType' || v.alias === 'entityType');
            if (entityTypePreValue.length > 0) {
                vm.entityType = entityTypePreValue[0].value;
            }
        }

        // set the available options by filtering out options not visible for the selected entityType
        vm.options = availableOptions.filter(o => o.visibleFor.indexOf(vm.entityType) > -1);
    }

    // listen to events from the entityType picker, and unsubscribe when destroying
    var unsubscribe = eventsService.on("commerceTools.entityTypeChanged", setAvailableOptions);
    $scope.$on("$destroy", function () {
        unsubscribe();
    });

    // initally set the available options
    setAvailableOptions();

}

angular.module('umbraco').controller("Umbraco.Cms.Integrations.Commerce.CommerceTools.PropertyEditors.OrderByPickerController", commerceToolsOrderByPickerController);