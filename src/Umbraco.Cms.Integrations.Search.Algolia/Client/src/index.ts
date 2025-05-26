import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifest as algoliaDashboardManifest } from './dashboard/manifest.js';
import { manifest as algoliaContextManifest } from './context/manifest.js';
import { client } from "@umbraco-integrations/algolia/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {

    extensionRegistry.registerMany([algoliaDashboardManifest, algoliaContextManifest]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        const config = auth?.getOpenApiConfiguration();

        client.setConfig({
            auth: config?.token ?? undefined,
            baseUrl: config?.base ?? "",
            credentials: config?.credentials ?? "same-origin",
        });
    });
};
