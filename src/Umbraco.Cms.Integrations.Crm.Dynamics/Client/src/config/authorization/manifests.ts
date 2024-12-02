import type { ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/property-editor";

export const manifests : ManifestPropertyEditorUi = {
    type: 'propertyEditorUi',
    alias: 'Dynamics.PropertyEditorUi.Authorization',
    name: 'Dynamics Form Picker Authorization Setting',
    js: () => import('./authorization-property-editor.element.js'),
    meta: {
        label: 'Authorization',
        icon: 'icon-autofill',
        group: 'common',
    }
}