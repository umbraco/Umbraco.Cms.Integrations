import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { client } from "@umbraco-integrations/semrush/generated";
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
        const config = auth?.getOpenApiConfiguration();

        client.setConfig({
            baseUrl: config?.base ?? "",
            auth: config?.token ?? undefined,
            credentials: config?.credentials ?? "same-origin",
        });
    });
  };