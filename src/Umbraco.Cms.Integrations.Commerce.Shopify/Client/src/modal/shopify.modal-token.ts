import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { ProductDtoModel } from "@umbraco-integrations/shopify/generated";
import { ShopifyProductPickerConfiguration } from "../types/types";

export type ShopifyProductPickerModalData = {
    headline: string;
    selectedItemIdList: Array<string | null>;
    config: ShopifyProductPickerConfiguration | undefined;
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