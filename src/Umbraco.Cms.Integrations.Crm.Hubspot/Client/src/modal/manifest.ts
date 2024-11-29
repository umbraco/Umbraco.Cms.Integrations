import type { ManifestModal } from "@umbraco-cms/backoffice/modal";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "HubspotForms.Modal",
    name: "Hubspot Forms Modal",
    js: () => import("./hubspot-forms-modal.element.js")
}