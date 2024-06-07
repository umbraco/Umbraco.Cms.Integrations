import { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "algolia.context",
    name: "Algolia Context",
    js: () => import("./algolia-index.context")
};

export const manifest = contextManifest;