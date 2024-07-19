import { UmbTableItem } from "@umbraco-cms/backoffice/components";
import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { UmbSelectionManager } from "@umbraco-cms/backoffice/utils";
import type { ProductDtoModel } from "@umbraco-integrations/shopify/generated";

export type ShopifyProductPickerModalData = {
    headline: string;
    selectedItemIdList: Array<string | null>;
}

export type ShopifyProductPickerModalValue = {
    productList: Array<ProductDtoModel>;
}

export const SHOPIFY_MODAL_TOKEN = new UmbModalToken<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>("Shopify.Modal", {
    modal: {
        type: "sidebar",
        size: "large"
    }
});