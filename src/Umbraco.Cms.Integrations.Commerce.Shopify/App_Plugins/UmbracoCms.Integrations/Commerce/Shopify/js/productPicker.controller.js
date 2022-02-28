function ProductPickerController($scope, editorService, notificationsService, umbracoCmsIntegrationsCommerceShopifyService, umbracoCmsIntegrationsCommerceShopifyResource) {

    const oauthName = "OAuth";

    var vm = this;

    vm.loading = false;
    vm.selectedProducts = [];

    vm.config = {
        validationLimit: {
            min: $scope.model.config.validationLimit.min ?? 0,
            max: $scope.model.config.validationLimit.max
        }
    }

    // step 1. check configuration
    checkConfiguration(function () {
        // step 2. get products
        getProducts();
    });

    function getProducts() {
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
    }

    // products table events
    vm.selectProduct = function (item) {

        var isProductSelected =
            $scope.model.selectedProducts.filter(function(i) { return i.id === item.id }).length > 0;

        // check if products count is in the validation limit interval
        var isProductsCountValid = validateProductsCount(isProductSelected);
        if (isProductsCountValid) {
            if (isProductSelected) {
                $scope.model.selectedProducts =
                    $scope.model.selectedProducts.filter(function(i) { return i.id !== item.id; });
            } else {
                $scope.model.selectedProducts.push(item);
            }
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
            config: vm.config,
            view: "/App_Plugins/UmbracoCms.Integrations/Commerce/Shopify/views/productPickerOverlay.html",
            size: "large",
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

    vm.submit = function (products) {
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

    function validateProductsCount(isRemoved) {

        var updatedCount = isRemoved
            ? $scope.model.selectedProducts.length - 1
            : $scope.model.selectedProducts.length + 1;

        if (vm.config.validationLimit.min != null && vm.config.validationLimit.max != null) {
            return vm.config.validationLimit.min <= updatedCount && vm.config.validationLimit.max >= updatedCount;
        }

        if (vm.config.validationLimit.min != null)
            return vm.config.validationLimit.min <= updatedCount;

        if (vm.config.validationLimit.max != null)
            return vm.config.validationLimit.max >= updatedCount;
    }
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Commerce.Shopify.ProductPickerController", ProductPickerController);