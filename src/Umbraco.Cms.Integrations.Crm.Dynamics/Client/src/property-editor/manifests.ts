import { ManifestTypes, type ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";
import { manifests as authorizationManifest } from "../config/authorization/manifests.js";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Dynamics.PropertyEditorUi.FormPicker",
    name: "Dynamics Form Picker Property Editor UI",
    js: () => import("./dynamics-form-picker-property-editor.element"),
    meta: {
        label: "Dynamics Form Picker",
        icon: "icon-handshake",
        group: "pickers",
		propertyEditorSchemaAlias: 'Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker',
        settings: {
            properties:[
                {
                    alias: 'configuration',
					label: 'Configuration',
					description: 'Connect with your Microsoft account.',
					propertyEditorUiAlias: 'Dynamics.PropertyEditorUi.Authorization',
                },
                {
                    alias: 'modules',
					label: 'Modules',
					description: 'Select the Microsoft Dynamics modules you want to use.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.RadioButtonList',
                    config: [{ alias: 'items', value: ['Outbound', 'RealTime', 'Both'] }]
                }
            ]
        }
    }
};

export const manifests : Array<ManifestTypes> = [
    propertyEditorUiManifest,
    authorizationManifest
];