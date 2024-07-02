import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "hubspot-forms.context",
    name: "Hubspot Forms Context",
    js: () => import("./hubspot-forms.context.js")
};

export const manifest = contextManifest;