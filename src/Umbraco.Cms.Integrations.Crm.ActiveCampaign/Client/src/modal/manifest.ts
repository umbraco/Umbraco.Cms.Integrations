import type { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

export const manifest: ManifestModal = {
    type: "modal",
    alias: "ActiveCampaignForms.Modal",
    name: "ActiveCampaign Forms Modal",
    js: () => import("./activecampaign-forms-modal.element.js")
}