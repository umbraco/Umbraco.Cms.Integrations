import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as dynamicsContext } from "./context/manifests";
import { manifests as dynamicsFormPicker } from "./property-editor/manifests";
import { OpenAPI } from "@umbraco-integrations/dynamics/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        dynamicsContext,
        ...dynamicsFormPicker
    ]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        const umbOpenApi = instance.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = true;
      });
}