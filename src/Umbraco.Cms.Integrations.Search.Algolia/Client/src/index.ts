import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifest as algoliaDashboardManifest } from './dashboard/manifest.js';
import { manifest as algoliaContextManifest } from './context/manifest.js';
import { OpenAPI } from "@umbraco-integrations/algolia/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {

    extensionRegistry.registerMany([algoliaDashboardManifest, algoliaContextManifest]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        const umbOpenApi = instance.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = true;
    });
};
