import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "algolia.context",
    name: "Algolia Context",
    js: () => import('./algolia-index.context.js')
};

export const manifest = contextManifest;