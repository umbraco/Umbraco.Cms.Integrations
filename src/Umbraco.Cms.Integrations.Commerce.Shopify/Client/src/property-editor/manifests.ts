import { ManifestTypes, type ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";
import { manifests as amountManifest } from "../config/amount/manifests.js";
import { manifests as authorizationManifest } from "../config/authorization/manifests.js";

export const propertyEditorUiManifest : ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Shopify.PropertyEditorUi.ProductPicker",
    name: "Shopify Product Picker Property Editor UI",
    element: () => import("./shopify-product-picker-property-editor.element.js"),
    meta: {
        label: "Shopify Product Picker",
        icon: "icon-shopping-basket-alt",
        group: "pickers",
		propertyEditorSchemaAlias: 'Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker',
        settings:{
            properties:[
                {
                    alias: 'authorization',
					label: 'Authorization',
					description: 'Authorize your Shopify connection.',
					propertyEditorUiAlias: 'Shopify.PropertyEditorUi.Authorization',
                },
                {
                    alias: 'minItems',
					label: 'Minimum number of items',
					description: 'Set a minimum number of items selected.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Integer'
                },
                {
                    alias: 'maxItems',
					label: 'Maximum number of items',
					description: 'Set a maximum number of items selected.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Integer'
                }
            ],
            defaultData:[
                { alias: 'minItems', value: 0 },
                { alias: 'maxItems', value: 2 },
            ]
        }
    }
};

export const manifests : Array<ManifestTypes> = [
    propertyEditorUiManifest,
    amountManifest,
    authorizationManifest
];