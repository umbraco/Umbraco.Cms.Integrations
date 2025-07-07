import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as zapierContext } from "./context/manifests";
import { manifests as zapierDashboard } from "./dashboard/manifests";
import { client } from "@umbraco-integrations/zapier/generated";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        zapierContext,
        ...zapierDashboard
    ]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        if (!auth) return;

        client.setConfig(umbHttpClient.getConfig());
    });
}