import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "zapier.context",
    name: "Zapier Context",
    js: () => import("./zapier.context")
};

export const manifest = contextManifest;