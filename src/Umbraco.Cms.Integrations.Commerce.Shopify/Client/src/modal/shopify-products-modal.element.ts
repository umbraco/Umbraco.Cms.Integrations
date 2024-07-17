import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context.js";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { html, css, state, customElement } from "@umbraco-cms/backoffice/external/lit";
import type { ProductDtoModel } from "../../generated";
import type { ShopifyProductPickerModalData, ShopifyProductPickerModalValue } from "./shopify.modal-token.js";
import type { ShopifyServiceStatus } from "../models/shopify-service.model.js";
import type { UmbTableColumn, UmbTableConfig, UmbTableItem, UmbTableSelectedEvent, UmbTableElement, UmbTableDeselectedEvent } from '@umbraco-cms/backoffice/components';
import type {ShopifyCollectionModel} from "../types/types.js";
import type { UmbDefaultCollectionContext } from '@umbraco-cms/backoffice/collection';
import { UMB_COLLECTION_CONTEXT } from '@umbraco-cms/backoffice/collection';

const elementName = "shopify-products-modal";

@customElement(elementName)
export default class ShopifyProductsModalElement extends UmbModalBaseElement<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>{
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;
    #collectionContext!: UmbDefaultCollectionContext<ShopifyCollectionModel>;

    @state()
	private _selection: Array<string> = [];

    @state()
	private _tableConfig: UmbTableConfig = {
		allowSelection: true,
	};

    @state()
	private _tableColumns: Array<UmbTableColumn> = [
		{
			name: 'Name',
			alias: 'productName'
		},
		{
			name: 'Vendor',
			alias: 'vendor',
		},
		{
			name: 'Status',
			alias: 'status'
		},
		{
			name: 'Tags',
			alias: 'tags'
		},
		{
			name: 'SKU',
			alias: 'sku',
		},
        {
			name: 'Barcode',
			alias: 'barcode',
		},
        {
			name: 'Price',
			alias: 'price',
		},
		{
			name: '',
			alias: 'entityActions'
		},
	];

    @state()
	private _tableItems: Array<UmbTableItem> = [];

    @state()
    private _serviceStatus: ShopifyServiceStatus = {
        isValid: false,
        type: "",
        description: "",
        useOAuth: false
    };

    @state()
    private _loading = false;

    @state()
    private _products: Array<ProductDtoModel> = [];

    constructor() {
        super();

        this.consumeContext(SHOPIFY_CONTEXT_TOKEN, (context) => {
            this.#shopifyContext = context;
        });

        this.consumeContext(UMB_COLLECTION_CONTEXT, (instance) => {
			this.#collectionContext = instance;
		});
    }

    async connectedCallback() {
        super.connectedCallback();
        this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        if (!this.#shopifyContext) return;

        const { data } = await this.#shopifyContext.checkConfiguration();
        if (!data || !data.type?.value) return;

        this._serviceStatus = {
            isValid: data.isValid,
            type: data.type.value,
            description: "",
            useOAuth: data.isValid && data.type.value === "OAuth"
        }

        await this.#loadProducts();
    }

    async #loadProducts() {
        this._loading = true;
        const { data } = await this.#shopifyContext.getList(undefined);
        if (!data) return;

        this._products = data.result.products ?? [];
        this._loading = false;

        if (!data.isValid || data.isExpired) {
            this._showError("Data is invalid or expired."!);
        }

        this.#createTableItems(this._products);
    }

    #onSelected(event: UmbTableSelectedEvent) {
		event.stopPropagation();
		const table = event.target as UmbTableElement;
		const selection = table.selection;
		this.#collectionContext?.selection.setSelection(selection);
	}

    #onDeselected(event: UmbTableDeselectedEvent) {
		event.stopPropagation();
		const table = event.target as UmbTableElement;
		const selection = table.selection;
		this.#collectionContext?.selection.setSelection(selection);
	}

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message },
        });
    }

    #createTableItems(products: Array<ProductDtoModel>) {
		this._tableItems = products.map((product) => {
			return {
                id: product.id.toString(),
				icon: 'icon-book-alt-2',
                data: [{
                    columnAlias: "productName",
                    value: product.title,
                },
                {
                    columnAlias: "vendor",
                    value: product.vendor,
                },
                {
                    columnAlias: "status",
                    value: product.status,
                },
                {
                    columnAlias: "tags",
                    value: product.tags,
                },
                {
                    columnAlias: "sku",
                    value: product.variants.map(v => v.sku).join(","),
                },
                {
                    columnAlias: "barcode",
                    value: product.variants.map(v => v.barcode).join(","),
                },
                {
                    columnAlias: "price",
                    value: product.variants[0].price,
                },
            ]
            }
		});
	}

    render() {
        return html`
            <umb-body-layout>
                <uui-box headline="Shopify Products">
                    ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    <umb-table 
                        .config=${this._tableConfig} 
                        .columns=${this._tableColumns} 
                        .items=${this._tableItems}
                        .selection=${this._selection}
                        @selected="${this.#onSelected}"
                        @deselected="${this.#onDeselected}"></umb-table>
                    
                </uui-box>
                <span>Add up to x items(s)</span>

                <uui-button slot="actions" label="Submit" @click=${this._submitModal}></uui-button>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }
}