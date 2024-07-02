import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifests as hubspotPropertyEditor } from "./property-editor/manifests";
import { manifest as hubspotContext } from "./context/manifest";
import { manifest as hubspotModal } from "./modal/manifest";

import { OpenAPI } from "@umbraco-integrations/hubspot-forms/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {

    extensionRegistry.registerMany([
        ...hubspotPropertyEditor,
        hubspotContext,
        hubspotModal
    ]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        const umbOpenApi = instance.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = true;
    });
};