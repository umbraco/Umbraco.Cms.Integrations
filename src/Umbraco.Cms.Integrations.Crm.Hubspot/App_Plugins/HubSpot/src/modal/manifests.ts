import type { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

const modalManifests: Array<ManifestModal> = [
 {
    type: "modal",
    alias: "HubSpot.FormPicker.Modal",
    name: "HubSpot Form Picker Modal",
    js: () => import("./elements/hubspot-forms-modal.element.js")    
 }
];

export const manifests = [...modalManifests];