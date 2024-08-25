import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifests as activeCampaignPropertyEditor } from "./property-editor/manifests";
import { manifest as activecampaignContext } from "./context/manifest.js";
import { manifest as activeCampaignModal } from "./modal/manifest.js";

import { OpenAPI } from "@umbraco-integrations/activecampaign-forms/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        ...activeCampaignPropertyEditor,
        activecampaignContext,
        activeCampaignModal
  ]);

  host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
    const umbOpenApi = instance.getOpenApiConfiguration();
    OpenAPI.TOKEN = umbOpenApi.token;
    OpenAPI.BASE = umbOpenApi.base;
    OpenAPI.WITH_CREDENTIALS = true;
  });
};
