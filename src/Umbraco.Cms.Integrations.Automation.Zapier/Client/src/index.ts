import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as zapierContext } from "./context/manifests";
import { manifests as zapierDashboard } from "./dashboard/manifests";
import { client } from "@umbraco-integrations/zapier/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        zapierContext,
        ...zapierDashboard
    ]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        const config = auth?.getOpenApiConfiguration();

        client.setConfig({
            auth: config?.token ?? undefined,
            baseUrl: config?.base ?? "",
            credentials: config?.credentials ?? "same-origin",
        });
    });
}