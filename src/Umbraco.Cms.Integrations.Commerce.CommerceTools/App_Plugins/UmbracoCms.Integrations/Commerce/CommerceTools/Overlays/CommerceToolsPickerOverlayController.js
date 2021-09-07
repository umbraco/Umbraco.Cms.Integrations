
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
function commerceToolsPickerOverlayController($scope, $q, $routeParams, commerceToolsResource, editorState, localizationService, editorService, overlayService) {

    var vm = this;

    vm.terms = "";
    vm.loading = false;
    vm.selectedItems = [];
    vm.items = [];
    vm.pagination = {
        pageNumber: 1,
        totalPages: 1
    }

    vm.columns = {
        variants: $scope.model.entityType === "Product",
        prices: $scope.model.entityType === "Product",
    }

    vm.options = {
        pageNumber: 1,
        pageSize: $scope.model.pageSize,
        orderBy: $scope.model.orderBy,
        orderDirection: $scope.model.orderDirection,
        languageCode: $routeParams.mculture ? $routeParams.mculture : null
    }

    vm.selectItem = function (item) {
        if ($scope.model.multiPicker) {

            if (vm.selectedItems.filter(function (i) { return i.id === item.id }).length > 0) {
                vm.selectedItems = vm.selectedItems.filter(function (i) { return i.id !== item.id; });
            }
            else {
                vm.selectedItems.push(item);
            }

        }
        else {
            $scope.model.submit(item.id);
        }
    }

    vm.close = function () {
        $scope.model.close();
    }

    vm.submit = function () {
        $scope.model.submit(vm.selectedItems.map(function (item) { return item.id }).join(','));
    }

    vm.isSelected = function (item) {
        return vm.selectedItems.filter(function (i) { return i.id === item.id }).length > 0;
    }


    vm.makeSearch = function () {
        vm.loading = true;

        commerceToolsResource.search(vm.terms, $scope.model.entityType, vm.options).then(function (data) {
            vm.pagination.pageNumber = data.pageIndex + 1;
            vm.pagination.totalPages = data.totalPages;
            vm.items = data.results;
            vm.items.forEach(function (item) {
                if ($scope.model.entityType === "Product") {
                    item.variantNames = getVariantNames(item);
                    item.prices = getPrices(item);
                    item.icon = "icon-box";
                }
                else if ($scope.model.entityType === "Category") {
                    item.icon = "icon-network-alt";
                }
            });
            vm.loading = false;
        });
    }

    $scope.onSearchStartTyping = function () {
        vm.loading = true;
    }

    vm.makeSearch();


    vm.isSortDirection = function (col, direction) {
        return vm.options.orderBy === col && vm.options.orderDirection === direction;
    };

    vm.sort = function (field) {
        if (vm.isSortDirection(field, vm.options.orderDirection)) {
            vm.options.orderDirection = vm.options.orderDirection === "asc" ? "desc" : "asc";
        }
        else {
            vm.options.orderBy = field;
            vm.options.orderDirection = "asc";
        }

        vm.pageNumber = 1;
        vm.makeSearch();
    };

    vm.nextPage = function (pageNumber) {
        vm.options.pageNumber = pageNumber;
        vm.makeSearch();
    };

    vm.goToPage = function (pageNumber) {
        vm.options.pageNumber = pageNumber;
        vm.makeSearch();
    };

    vm.prev = function (pageNumber) {
        vm.options.pageNumber = pageNumber;
        vm.makeSearch();
    };

    var getVariantNames = function (item) {
        return item.variants.map(function (variant) { return variant.sku }).join(', ');
    };

    var getPrices = function (item) {
        var currencies = [];
        item.variants.map(function (variant) {

            variant.prices.map(function (price) {
                if (currencies.indexOf(price.currencyCode) === -1) {
                    currencies.push(price.currencyCode);
                }
            });
        });

        if (currencies.length === 0) {
            return "";
        }

        var pricesForCurrency = [];
        item.variants.map(function (variant) {
            variant.prices
                .filter(function (price) { return price.currencyCode === currencies[0] })
                .map(function (price) { pricesForCurrency.push(price.centAmount) });
        });

        pricesForCurrency = pricesForCurrency.sort();

        if (pricesForCurrency.length === 0) {
            return "";
        }
        else if (pricesForCurrency.length === 1) {
            return pricesForCurrency[0] + ' ' + currencies[0];
        }

        return pricesForCurrency[0] + ' - ' + pricesForCurrency[pricesForCurrency.length - 1] + ' ' + currencies[0];
    }

}

angular.module('umbraco').controller("Umbraco.Cms.Integrations.Commerce.CommerceTools.Overlays.CommerceToolsPickerOverlayController", commerceToolsPickerOverlayController);