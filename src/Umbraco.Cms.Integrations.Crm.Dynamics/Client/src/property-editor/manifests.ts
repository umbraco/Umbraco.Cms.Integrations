import { ManifestTypes, type ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Dynamics.PropertyEditorUi.FormPicker",
    name: "Dynamics Form Picker Property Editor UI",
    element: () => import("./dynamics-form-picker-property-editor.element"),
    meta: {
        label: "Dynamics Form Picker",
        icon: "icon-book",
        group: "pickers",
		propertyEditorSchemaAlias: 'Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker',
    }
};

export const manifests : Array<ManifestTypes> = [
    propertyEditorUiManifest
];