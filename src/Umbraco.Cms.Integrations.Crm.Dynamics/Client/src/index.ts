import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as dynamicsContext } from "./context/manifests";
import { manifests as dynamicsFormPicker } from "./property-editor/manifests";
import { manifest as dynamicsModal } from "./modal/manifests";
import { client } from "@umbraco-integrations/dynamics/generated";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        dynamicsContext,
        ...dynamicsFormPicker,
        dynamicsModal
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

export * from './config/authorization/authorization-property-editor.element';