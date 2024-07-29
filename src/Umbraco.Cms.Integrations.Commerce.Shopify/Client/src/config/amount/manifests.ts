import type { ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const manifests : ManifestPropertyEditorUi = {
    type: 'propertyEditorUi',
	alias: 'Shopify.PropertyEditorUi.Amount',
	name: 'Shopify Product Picker Amount Setting',
	element: () => import('./amount-property-editor.element.js'),
	meta: {
		label: 'Amount',
		icon: 'icon-autofill',
		group: 'common',
    }
};