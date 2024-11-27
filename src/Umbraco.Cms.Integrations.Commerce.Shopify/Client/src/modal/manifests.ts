import type { ManifestModal } from "@umbraco-cms/backoffice/modal";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "Shopify.Modal",
    name: "Shopify Modal",
    js: () => import("./shopify-products-modal.element")
}