import type { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contextManifest: ManifestGlobalContext = {
    type: "globalContext",
    alias: "dynamics.context",
    name: "Dynamics Context",
    js: () => import("./dynamics.context")
};

export const manifest = contextManifest;