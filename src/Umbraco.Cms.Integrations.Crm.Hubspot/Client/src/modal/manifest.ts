import type { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "HubspotForms.Modal",
    name: "Hubspot Forms Modal",
    js: () => import("./hubspot-forms-modal.element.js")
}