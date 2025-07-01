import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { client } from "@umbraco-integrations/semrush/generated";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";
import { manifest as semrushContext } from "./context/manifest";
import { manifests as workspaceManifest } from "./workspace/manifests";
import { manifest as modalManifest } from "./modal/manifest";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        semrushContext,
        modalManifest,
        ...workspaceManifest
    ]);
  
    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        if (!auth) return;

        client.setConfig(umbHttpClient.getConfig());
    });
  };