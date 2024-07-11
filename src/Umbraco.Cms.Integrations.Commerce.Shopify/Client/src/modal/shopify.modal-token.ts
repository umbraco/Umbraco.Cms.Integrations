import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { ProductDtoModel } from "@umbraco-integrations/shopify/generated";

export type ShopifyProductPickerModalData = {
    headline: string;
}

export type ShopifyProductPickerModalValue = {
    products: ProductDtoModel;
}

export const SHOPIFY_MODAL_TOKEN = new UmbModalToken<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>("Shopify.Modal", {
    modal: {
        type: "sidebar",
        size: "small"
    }
});