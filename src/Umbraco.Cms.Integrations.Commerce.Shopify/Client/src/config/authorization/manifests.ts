import type { ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const manifests : ManifestPropertyEditorUi = {
    type: 'propertyEditorUi',
    alias: 'Shopify.PropertyEditorUi.Authorization',
    name: 'Shopify Product Picker Authorization Setting',
    element: () => import('./authorization-property-editor.element.js'),
    meta: {
        label: 'Authorization',
        icon: 'icon-autofill',
        group: 'common',
    }
}