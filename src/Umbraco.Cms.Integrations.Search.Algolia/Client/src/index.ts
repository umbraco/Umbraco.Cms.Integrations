import { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { OpenAPI } from "@umbraco-integrations/algolia/generated";

import { manifest as algoliaDashboardManifest } from "./dashboard/manifest.ts";
import { manifest as algoliaContextManifest } from "./context/manifest.ts";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {

    extensionRegistry.registerMany([algoliaDashboardManifest, algoliaContextManifest]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        const umbOpenApi = instance.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = true;
    });
};
