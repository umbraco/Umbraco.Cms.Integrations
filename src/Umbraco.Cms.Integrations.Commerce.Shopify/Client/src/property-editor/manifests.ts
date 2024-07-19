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
                    alias: 'amount',
					label: 'Amount',
					description: 'Set a required range of items selected.',
					propertyEditorUiAlias: 'Shopify.PropertyEditorUi.Amount',
                    config: [
                        {
                            alias: 'amountMin',
                            value: 0,
                        },{
                            alias: 'amountMax',
                            value: 2,
                        }
                    ],
                }
            ]
        }
    }
};

export const manifests : Array<ManifestTypes> = [
    propertyEditorUiManifest,
    amountManifest,
    authorizationManifest
];