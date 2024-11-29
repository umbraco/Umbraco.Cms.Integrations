import type { ManifestModal } from "@umbraco-cms/backoffice/modal";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "Dynamics.Modal",
    name: "Dynamics Modal",
    js: () => import("./dynamics-form-modal.element")
}