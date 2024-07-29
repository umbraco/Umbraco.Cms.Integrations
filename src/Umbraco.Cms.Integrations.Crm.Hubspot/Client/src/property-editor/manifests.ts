import type { ManifestPropertyEditorUi, ManifestPropertyEditorSchema } from "@umbraco-cms/backoffice/extension-registry";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "HubSpot.PropertyEditorUi.FormPicker",
    name: "HubSpot Form Picker Property Editor UI",
    js: () => import("./form-picker-property-editor.element.js"),
    elementName: "hubspot-form-picker",
    meta: {
        label: "HubSpot Form Picker",
        icon: "icon-handshake",
        group: "pickers",
        propertyEditorSchemaAlias: "HubSpot.FormPicker"
    }
};

const propertyEditorSchema : ManifestPropertyEditorSchema = {
    type: "propertyEditorSchema",
    name: "HubSpot Form Picker",
    alias: "HubSpot.FormPicker",
    meta: {
        defaultPropertyEditorUiAlias: "HubSpot.PropertyEditorUi.FormPicker",
        settings: {
            properties: [
                {
                    alias: "hubspot.authorization",
                    label: "Authorization",
                    description: "Authorization Details",
                    propertyEditorUiAlias: "HubSpot.PropertyEditorUi.Authorization"
                }
            ]
        }
    }
};

const authorizationPropertyEditorUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "HubSpot.PropertyEditorUi.Authorization",
    name: "HubSpot Authorization Property Editor UI",
    js: () => import("./authorization-property-editor.element.js"),
    elementName: "hubspot-authorization",
    meta: {
        label: "Authorization",
        icon: "",
        group: ""
    }
}

export const manifests = [
    propertyEditorUiManifest,
    propertyEditorSchema,
    authorizationPropertyEditorUiManifest
];

