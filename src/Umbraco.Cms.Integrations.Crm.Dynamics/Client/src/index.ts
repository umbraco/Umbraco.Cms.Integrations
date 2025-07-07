import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { manifest as dynamicsContext } from "./context/manifests";
import { manifests as dynamicsFormPicker } from "./property-editor/manifests";
import { manifest as dynamicsModal } from "./modal/manifests";
import { client } from "@umbraco-integrations/dynamics/generated";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany([
        dynamicsContext,
        ...dynamicsFormPicker,
        dynamicsModal
    ]);

    host.consumeContext(UMB_AUTH_CONTEXT, async (auth) => {
        if (!auth) return;

        client.setConfig(umbHttpClient.getConfig());
    });
}

export * from './config/authorization/authorization-property-editor.element';