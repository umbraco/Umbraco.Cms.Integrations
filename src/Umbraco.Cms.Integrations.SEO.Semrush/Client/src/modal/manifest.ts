import type { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "Semrush.Modal",
    name: "Semrush Modal",
    js: () => import("./semrush-modal.element")
}