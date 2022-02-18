function ProductPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCommerceShopifyService, umbracoCmsIntegrationsCommerceShopifyResource) {

    const oauthName = "OAuth";

    var vm = this;

    vm.loading = false;
    vm.selectedProducts = [];

    // step 1. check configuration
    checkConfiguration(function() {
        // step 2. get products
        vm.loading = true;
        umbracoCmsIntegrationsCommerceShopifyResource.getProductsList().then(function (response) {
            if (response.isValid) {
                vm.productsList = response.result.products;

                if ($scope.model.value != undefined && $scope.model.value.length > 0) {
                    loadProductsPreview();
                };
            }
            vm.loading = false;
        });
    });

    vm.selectProduct = function (item) {
        if ($scope.model.selectedProducts.filter(function (i) { return i.id === item.id }).length > 0) {
            $scope.model.selectedProducts = $scope.model.selectedProducts.filter(function (i) { return i.id !== item.id; });
        }
        else {
            $scope.model.selectedProducts.push(item);
        }
    }

    vm.isSelected = function (item, products) {
        return products.filter(function (i) { return i.id === item.id }).length > 0;
    }

    vm.openProductsPickerOverlay = function () {
        var options = {
            title: "Shopify products",
            description: "Select product(s)",
            selectedProducts: vm.selectedProducts,
            view: "/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPickerOverlay.html",
            size: "medium",
            submit: function (selectedProducts) {
                vm.submit(selectedProducts);

                loadProductsPreview();

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(options);
    }

    vm.submit = function(products) {
        $scope.model.value = products.map(function (item) { return item.id }).join(',');
    }

    vm.remove = function (node) {
        vm.submit(vm.selectedProducts.filter(el => el.id != node.alias));
        loadProductsPreview();
    }

    function checkConfiguration(callback) {
        umbracoCmsIntegrationsCommerceShopifyResource.checkConfiguration().then(function (response) {

            vm.status = {
                isValid: response.isValid === true,
                type: response.type,
                description: umbracoCmsIntegrationsCommerceShopifyService.configDescription[response.type.value],
                useOAuth: response.isValid === true && response.type.value === oauthName
            };

            if (response.isValid === false) {
                vm.loading = false;
                vm.error = umbracoCmsIntegrationsCommerceShopifyService.configDescription.None;
                notificationsService.warning("Shopify API", umbracoCmsIntegrationsCommerceShopifyService.configDescription.None);
            } else {
                callback();
            }
        });
    }

    function loadProductsPreview() {
        vm.previewNodes = [];

        var ids = $scope.model.value.split(",");

        var list = vm.productsList.filter(el => {
            var id = ids.find(e => e == el.id);
            return id !== undefined ? el : null;
        });
        vm.selectedProducts = list;

        list.forEach(el => {
            vm.previewNodes.push({
                icon: "icon-shopping-basket",
                name: el.title,
                alias: el.id
            });
        });
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Commerce.Shopify.ProductPickerController", ProductPickerController);