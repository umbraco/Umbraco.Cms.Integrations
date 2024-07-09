import type { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "Shopify.Modal",
    name: "Shopify Modal",
    js: () => import("./shopify-modal.element")
}