import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import type { ManifestTypes } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifests as modalManifests } from "./modal/manifests.js";

export * from './modal/index.js';

const manifests: Array<ManifestTypes> = [
    ...modalManifests
];

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany(manifests);

    
  host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
    if (!instance) return;
  });
};