import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { manifests as hubspotPropertyEditor } from "./property-editor/manifests.js";
import { manifest as hubspotContext } from "./context/manifest.js";
import { manifest as hubspotModal } from "./modal/manifest.js";

import { client } from "@umbraco-integrations/hubspot-forms/generated";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";

export * from "./property-editor/index.js";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
  extensionRegistry.registerMany([
    ...hubspotPropertyEditor,
    hubspotContext,
    hubspotModal,
  ]);

  host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
      if (!auth) return;

      client.setConfig(umbHttpClient.getConfig());
  });
};
