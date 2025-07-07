import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as shopifyContext } from "./context/manifests";
import { manifests as picker } from "./property-editor/manifests.js";
import { manifest as shopifyModal } from "./modal/manifests.js";
import { client } from "@umbraco-integrations/shopify/generated";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        ...picker,
        shopifyModal,
        shopifyContext
    ]);
  
    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        if (!auth) return;

        client.setConfig(umbHttpClient.getConfig());
    });
  };