function commerceToolsEntityTypePickerController($scope, eventsService) {
    // listen for changes, and emit event, to make the orderby picker update its options
    $scope.$watch("model.value", function (newVal, oldVal) {
        eventsService.emit("commerceTools.entityTypeChanged");
    })
}

angular.module('umbraco').controller("Umbraco.Cms.Integrations.Commerce.CommerceTools.PropertyEditors.EntityTypePickerController", commerceToolsEntityTypePickerController);