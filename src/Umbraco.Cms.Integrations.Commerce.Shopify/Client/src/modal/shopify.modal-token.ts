import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { ProductDtoModel } from "../generated";

export type ShopifyPickerModalData = {
    headline: string;
}

export type ShopifyPickerModalValue = {
    products: ProductDtoModel;
}

export const SHOPIFY_MODAL_TOKEN = new UmbModalToken<ShopifyPickerModalData, ShopifyPickerModalValue>("Shopify.Modal", {
    modal: {
        type: "sidebar",
        size: "small"
    }
});