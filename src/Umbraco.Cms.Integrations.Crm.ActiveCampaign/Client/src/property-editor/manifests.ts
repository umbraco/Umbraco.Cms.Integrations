import type { ManifestPropertyEditorUi, ManifestPropertyEditorSchema, ManifestIcons } from "@umbraco-cms/backoffice/extension-registry";

export const propertyEditorUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "ActiveCampaign.PropertyEditorUi.FormPicker",
    name: "ActiveCampaign Form Picker Property Editor UI",
    js: () => import("./form-picker-property-editor.element.js"),
    elementName: "activecampaign-form-picker",
    meta: {
        label: "ActiveCampaign Form Picker",
        icon: "icon-activecampaign",
        group: "pickers",
        propertyEditorSchemaAlias: "ActiveCampaign.FormPicker"
    }
};

const propertyEditorSchema: ManifestPropertyEditorSchema = {
    type: "propertyEditorSchema",
    name: "ActiveCampaign Form Picker",
    alias: "ActiveCampaign.FormPicker",
    meta: {
        defaultPropertyEditorUiAlias: "ActiveCampaign.PropertyEditorUi.FormPicker",
        settings: {
            properties: [
                {
                    alias: "activecampaign.configuration",
                    label: "Configuration",
                    description: "API Access",
                    propertyEditorUiAlias: "ActiveCampaign.PropertyEditorUi.Configuration"
                }
            ]
        }
    }
};

const configurationPropertyEditorUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "ActiveCampaign.PropertyEditorUi.Configuration",
    name: "ActiveCampaign Configuration Property Editor UI",
    js: () => import("./configuration-property-editor.element.js"),
    elementName: "activecampaign-forms-configuration",
    meta: {
        label: "Configuration",
        icon: "",
        group: ""
    }
}

const propertyEditorIcon: ManifestIcons = {
    type: "icons",
    name: "ActiveCampaign Forms Icon",
    alias: "ActiveCampaign.PropertyEditorUi.Icon",
    js: () => import("../icons/icons-dictionary.js")
}

export const manifests = [
    propertyEditorUiManifest,
    propertyEditorSchema,
    configurationPropertyEditorUiManifest,
    propertyEditorIcon
];