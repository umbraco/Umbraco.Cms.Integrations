function SemrushToolsController($scope, editorState, contentResource) {
    var vm = this;

    vm.CurrentNodeId = editorState.current.id;
    vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

    contentResource.getById(vm.CurrentNodeId).then(function (node) {
        var properties = node.variants[0].tabs[0].properties;

        vm.CurrentNodeProperties = properties;
    });

    vm.OnPropertyChange = function () {
        vm.SelectedPropertyDetails = vm.CurrentNodeProperties.find(obj => {
            return obj.alias == vm.SelectedProperty;
        });
    }
}
angular.module('umbraco')
    .controller('Umbraco.Cms.Integrations.SEO.SemrushTools.SemrushToolsController', SemrushToolsController);
