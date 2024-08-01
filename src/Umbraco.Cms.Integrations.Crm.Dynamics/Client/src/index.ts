import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifests as dynamicsPropertyEditor } from "./property-editor/manifests.js";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([...dynamicsPropertyEditor]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (_instance) => {
        //const umbOpenApi = instance.getOpenApiConfiguration();
        //OpenAPI.TOKEN = umbOpenApi.token;
        //OpenAPI.BASE = umbOpenApi.base;
        //OpenAPI.WITH_CREDENTIALS = true;
    });
};
