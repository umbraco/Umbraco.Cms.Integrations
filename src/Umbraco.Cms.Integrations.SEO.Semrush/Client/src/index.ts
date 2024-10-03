import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { OpenAPI } from "@umbraco-integrations/semrush/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([

    ]);
  
    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
      const umbOpenApi = instance.getOpenApiConfiguration();
      OpenAPI.TOKEN = umbOpenApi.token;
      OpenAPI.BASE = umbOpenApi.base;
      OpenAPI.WITH_CREDENTIALS = true;
    });
  };