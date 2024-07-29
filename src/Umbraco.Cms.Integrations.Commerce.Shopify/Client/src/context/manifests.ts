import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "shopify.context",
    name: "Shopify Context",
    js: () => import("./shopify.context")
};

export const manifest = contextManifest;