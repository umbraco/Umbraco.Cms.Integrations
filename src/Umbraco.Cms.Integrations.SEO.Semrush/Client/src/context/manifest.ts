import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "semrush.context",
    name: "Semrush Context",
    js: () => import("./semrush.context")
};

export const manifest = contextManifest;