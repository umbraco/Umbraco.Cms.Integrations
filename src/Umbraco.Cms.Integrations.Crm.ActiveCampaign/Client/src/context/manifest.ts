import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "activecampaign-forms.context",
    name: "ActiveCampaign Forms Context",
    js: () => import("./activecampaign-forms.context.js")
};

export const manifest = contextManifest;