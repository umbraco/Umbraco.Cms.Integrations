import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context.js";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { html, state, customElement, css } from "@umbraco-cms/backoffice/external/lit";
import type { EditorSettingsModel, ProductDtoModel } from "../../generated";
import type { ShopifyProductPickerModalData, ShopifyProductPickerModalValue } from "./shopify.modal-token.js";
import type { ShopifyServiceStatus } from "../models/shopify-service.model.js";
import type { UmbTableColumn, UmbTableConfig, UmbTableItem, UmbTableSelectedEvent, UmbTableElement, UmbTableDeselectedEvent, UmbTableItemData } from '@umbraco-cms/backoffice/components';
import type { ShopifyCollectionModel } from "../types/types.js";
import type { UmbDefaultCollectionContext } from '@umbraco-cms/backoffice/collection';
import { UMB_COLLECTION_CONTEXT } from '@umbraco-cms/backoffice/collection';

const elementName = "shopify-products-modal";

@customElement(elementName)
export default class ShopifyProductsModalElement extends UmbModalBaseElement<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>{
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;
    #settingsModel?: EditorSettingsModel;
    #collectionContext!: UmbDefaultCollectionContext<ShopifyCollectionModel>;
    _modalSelectedProducts: Array<ProductDtoModel> = [];
    _numberOfSelection: number = 0;
    _addUpToItems: number = 0;
    _selectionIdList: Array<string | null> = [];
    
    @state()
	private _selection: Array<string | null> = [];

    @state()
	private _tableConfig: UmbTableConfig = {
		allowSelection: true,
	};

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

    constructor() {
        super();

        this.consumeContext(SHOPIFY_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#shopifyContext = context;
            this.observe(context.settingsModel, (settingsModel) => {
                this.#settingsModel = settingsModel;
            });
        });

        this.consumeContext(UMB_COLLECTION_CONTEXT, (instance) => {
			this.#collectionContext = instance;
            this.observe(
				this.#collectionContext.selection.selection,
				(selection) => (this._selection = selection),
				'umbCollectionSelectionObserver',
			);
		});
    }

    async connectedCallback() {
        super.connectedCallback();
        this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        if (!this.#shopifyContext || !this.#settingsModel) return;

        this._serviceStatus = {
            isValid: this.#settingsModel.isValid,
            type: this.#settingsModel.type.value,
            description: "",
            useOAuth: this.#settingsModel.isValid && this.#settingsModel.type.value === "OAuth"
        }

        await this.#loadProducts();
    }

    async #loadProducts() {
        this._loading = true;
        const { data } = await this.#shopifyContext.getList("");
        if (!data) return;

        this._products = data.result.products ?? [];
        this._loading = false;

        if (!data.isValid || data.isExpired) {
            this._showError("Data is invalid or expired."!);
        }

        this.#createTableItems(this._products);
        this.#loadSelectionItems();
    }

    #createTableItems(products: Array<ProductDtoModel>) {
		this._tableItems = products.map((product) => {
			return {
                id: product.id.toString(),
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

    async #loadSelectionItems(){
        this._selection = this.data!.selectedItemIdList;
        this._addUpToItems = this.data?.config?.maxItems! - this.data?.config?.minItems!;
    }

    #onSelected(event: UmbTableSelectedEvent) {
        this.#onEventRun(event);
	}

    #onDeselected(event: UmbTableDeselectedEvent) {
		this.#onEventRun(event);
	}

    #onEventRun(event: UmbTableSelectedEvent | UmbTableDeselectedEvent){
        event.stopPropagation();
		const table = event.target as UmbTableElement;
		const selection = table.selection;
        const items = table.items;
		this.#collectionContext?.selection.setSelection(selection);

        this.#getSelectedProduct(selection, items);
        this._numberOfSelection = selection.length;
    }

    #getSelectedProduct(selectedRows: Array<string>, allRows: Array<UmbTableItem>){
        let lst: Array<UmbTableItem[]> = [];
        selectedRows.forEach(selectedRow => {
            const selectedProduct = allRows.filter(r => r.id == selectedRow);
            lst.push(selectedProduct); 
        });

        let lstData = lst.map(l => l[0].data);
        let lstId = lst.map(l => l[0].id);
        this._modalSelectedProducts = this.#mapToDto(lstData, lstId);
    }

    #mapToDto(lstData: UmbTableItemData[][], lstId: string[]){
        let productList: Array<ProductDtoModel> = [];
        for(let i = 0; i < lstData.length; i++){
            let dto: ProductDtoModel = {
                title: lstData[i].find(x => x.columnAlias == "productName")?.value,
                vendor: lstData[i].find(x => x.columnAlias == "vendor")?.value,
                id: Number(lstId[i]),
                body: "",
                status: lstData[i].find(x => x.columnAlias == "status")?.value,
                tags: lstData[i].find(x => x.columnAlias == "tags")?.value,
                variants: [],
                image: {
                    src: ""
                }
            }

            productList.push(dto);
        }

        return productList;
    }

    _onSubmit() {
        if(this._numberOfSelection > this.data?.config?.maxItems! || this._numberOfSelection < this.data?.config?.minItems!){
            this._showError("Please select the amount of items that has been configured in the setting.");
        }else{
            if(this._numberOfSelection == 0){
                this._rejectModal();
            }

            this.value = {productList: this._modalSelectedProducts};
            this._submitModal();
        }
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message },
        });
    }

    render() {
        return html`
            <umb-body-layout>
                <uui-box headline=${this.data!.headline}>
                    ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    <umb-table 
                        .config=${this._tableConfig} 
                        .columns=${this._tableColumns} 
                        .items=${this._tableItems}
                        .selection=${this._selection}
                        @selected="${this.#onSelected}"
                        @deselected="${this.#onDeselected}"></umb-table>
                    
                </uui-box>

                <div class="maximum-selection">
                    <span>
                        Add up to ${this._addUpToItems} items(s)
                    </span>
                </div>

                <uui-button look="primary"  slot="actions" label="Submit" @click=${this._onSubmit}></uui-button>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [css`
        .maximum-selection{
            margin-top: 10px;
            font-weight: bold;
        }
    `];
}