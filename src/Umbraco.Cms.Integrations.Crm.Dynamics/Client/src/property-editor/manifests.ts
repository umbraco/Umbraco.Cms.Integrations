import type { ManifestPropertyEditorUi, ManifestPropertyEditorSchema } from "@umbraco-cms/backoffice/extension-registry";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Dynamics.PropertyEditorUi.FormPicker",
    name: "Dynamics Form Picker Property Editor UI",
    js: () => import("./form-picker-property-editor.element.js"),
    elementName: "dynamics-form-picker",
    meta: {
        label: "Dynamics Form Picker",
        icon: "icon-handshake",
        group: "pickers",
        propertyEditorSchemaAlias: "Dynamics.FormPicker"
    }
};

const propertyEditorSchema: ManifestPropertyEditorSchema = {
    type: "propertyEditorSchema",
    name: "Dynamics Form Picker",
    alias: "Dynamics.FormPicker",
    meta: {
        defaultPropertyEditorUiAlias: "Dynamics.PropertyEditorUi.FormPicker",
        settings: {
            properties: [
                {
                    alias: "dynamics.configuration",
                    label: "Authorization",
                    description: "Authorization Details",
                    propertyEditorUiAlias: "Dynamics.PropertyEditorUi.Configuration"
                }
            ]
        }
    }
};

const configurationPropertyEditorUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Dynamics.PropertyEditorUi.Configuration",
    name: "Dynamics Configuration Property Editor UI",
    js: () => import("./configuration-property-editor.element.js"),
    elementName: "dynamics-configuration",
    meta: {
        label: "Authorization",
        icon: "",
        group: ""
    }
}

export const manifests = [
    propertyEditorUiManifest,
    propertyEditorSchema,
    configurationPropertyEditorUiManifest
];

