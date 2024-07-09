import type { ManifestPropertyEditorUi, ManifestPropertyEditorSchema } from "@umbraco-cms/backoffice/extension-registry";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Shopify.PropertyEditorUi.ProductPicker",
    name: "Shopify Product Picker Property Editor UI",
    js: () => import("./shopify-picker-property-editor.element.js"),
    elementName: "shopify-picker",
    meta: {
        label: "Shopify Product Picker",
        icon: "icon-handshake",
        group: "pickers",
        propertyEditorSchemaAlias: "Shopify.ProductPicker"
    }
};

const propertyEditorSchema : ManifestPropertyEditorSchema = {
    type: "propertyEditorSchema",
    name: "Shopify Product Picker",
    alias: "Shopify.ProductPicker",
    meta: {
        defaultPropertyEditorUiAlias: "Shopify.PropertyEditorUi.ProductPicker",
        settings: {
            properties: [
                {
                    alias: "Shopify.authorization",
                    label: "Authorization",
                    description: "Authorization Details",
                    propertyEditorUiAlias: "Shopify.PropertyEditorUi.Authorization"
                }
            ]
        }
    }
};

export const manifests = [
    propertyEditorUiManifest,
    propertyEditorSchema
];