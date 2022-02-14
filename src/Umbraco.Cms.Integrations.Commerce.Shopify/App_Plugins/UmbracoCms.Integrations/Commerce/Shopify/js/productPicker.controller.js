function ProductPickerController($scope, umbracoCmsIntegrationsCommerceShopifyResource) {

    var vm = this;

    umbracoCmsIntegrationsCommerceShopifyResource.getProductsList().then(function(response) {
        console.log(response);
    });

}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Commerce.Shopify.ProductPickerController", ProductPickerController);