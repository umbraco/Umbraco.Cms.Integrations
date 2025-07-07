import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context.js";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { html, state, customElement, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import type { EditorSettingsModelReadable, ProductDtoModel } from "../../generated";
import type { ShopifyProductPickerModalData, ShopifyProductPickerModalValue } from "./shopify.modal-token.js";
import type { ShopifyServiceStatus } from "../models/shopify-service.model.js";
import type { UmbTableColumn, UmbTableConfig, UmbTableItem, UmbTableSelectedEvent, UmbTableElement, UmbTableDeselectedEvent, UmbTableItemData } from '@umbraco-cms/backoffice/components';
import type { ShopifyCollectionModel } from "../types/types.js";
import type { UmbDefaultCollectionContext } from "@umbraco-cms/backoffice/collection";
import { UMB_COLLECTION_CONTEXT } from "@umbraco-cms/backoffice/collection";
import { UmbPaginationManager } from "@umbraco-cms/backoffice/utils";
import type { UUIPaginationEvent } from "@umbraco-cms/backoffice/external/uui";

const elementName = "shopify-products-modal";

@customElement(elementName)
export default class ShopifyProductsModalElement extends UmbModalBaseElement<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>{
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;
    #settingsModel?: EditorSettingsModelReadable;
    #collectionContext!: UmbDefaultCollectionContext<ShopifyCollectionModel>;
    #paginationManager = new UmbPaginationManager();
    _modalSelectedProducts: Array<ProductDtoModel> = [];
    _numberOfSelection: number = 0;
    _maximumItems: number = 0;
    _minimumItems: number = 0;
    _selectionIdList: Array<string | null> = [];

    @state()
    _currentPageNumber = 1;

    @state()
    _totalPages = 1;

    @state()
    _nextPageInfo?: string;

    @state()
    _previousPageInfo?: string;
    
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

    private _selectedItems: Array<string | null> = [];
    private _selectedProducts: Array<ProductDtoModel> = [];

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
            if (!instance) return;

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

        if (!this._serviceStatus.isValid) {
            this._showError("Invalid Shopify API Configuration");
            return;
        }

        await this.#getTotalPages();
        await this.#loadProducts("");
    }

    async #loadProducts(pageInfo?: string) {
        await this.#getTotalPages();

        this._loading = true;
        const { data } = await this.#shopifyContext.getList(pageInfo);
        if (!data) return;

        if (!data.isValid) {
            this._showError("Cannot access Shopify API.");
            this._loading = false;
            return;
        }

        this._products = data.result.products ?? [];
        this._loading = false;

        if (!data.isValid || data.isExpired) {
            this._showError("Data is invalid or expired."!);
        }

        this._nextPageInfo = data.nextPageInfo;
        this._previousPageInfo = data.previousPageInfo;

        this.#createTableItems(this._products);
        this.#loadSelectionItems();
    }

    async #getTotalPages() {
        const { data } = await this.#shopifyContext.getTotalPages();
        if (!data) return;

        this._totalPages = data;
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

    async #loadSelectionItems() {
        this._selection = this._selectedItems.length > 0
            ? this._selectedItems
            : this.data!.selectedItemIdList;
        this._maximumItems = this.data?.config?.maxItems ?? 0;
        this._minimumItems = this.data?.config?.minItems ?? 0;
    }

    #onSelected(event: UmbTableSelectedEvent) {
        this.#onEventRun(event);
	}

    #onDeselected(event: UmbTableDeselectedEvent) {
		this.#onEventRun(event);
	}

    #onEventRun(event: UmbTableSelectedEvent | UmbTableDeselectedEvent) {

        event.stopPropagation();
		const table = event.target as UmbTableElement;
		const selection = table.selection;
        const items = table.items;

        this.saveSelectedItems(items, selection);

        this.#collectionContext?.selection.setSelection(selection);

        this.#getSelectedProduct(selection, items);
        this._numberOfSelection = selection.length;
    }

    private saveSelectedItems(items: UmbTableItem[], selection: string[]) {
        // remove current table view items from the selected array to cover the deselect action.
        this._selectedItems = this._selectedItems.filter(obj => {
            if (!items.some(item => item.id == obj)) {
                return obj;
            }
        });
        selection.forEach(obj => {
            if (this._selectedItems.indexOf(obj) == -1) {
                this._selectedItems.push(obj);
            }
        });
    }

    #getSelectedProduct(selectedRows: Array<string>, allRows: Array<UmbTableItem>){
        let lst: Array<UmbTableItem[]> = [];
        selectedRows.forEach(selectedRow => {
            const selectedProduct = allRows.filter(r => r.id == selectedRow);
            if (selectedProduct && selectedProduct.length > 0) {
                lst.push(selectedProduct);
            }
        });

        let lstData = lst.map(l => l[0].data);
        let lstId = lst.map(l => l[0].id);
        this._modalSelectedProducts = this.#mapToDto(lstData, lstId);

        this.saveSelectedProducts(allRows);
    }

    private saveSelectedProducts(allRows: Array<UmbTableItem>) {
        // clear items of current table view
        this._selectedProducts = this._selectedProducts.filter(obj => {
            if (!allRows.some(row => row.id == obj.id.toString())) {
                return obj;
            }
        });

        this._modalSelectedProducts.forEach(obj => {
            if (!this._selectedProducts.some(product => product.id == obj.id)) {
                this._selectedProducts.push(obj);
            }
        });
    }

    #mapToDto(lstData: UmbTableItemData[][], lstId: string[]){
        let productList: Array<ProductDtoModel> = [];
        for(let i = 0; i < lstData.length; i++){
            let dto: ProductDtoModel = {
                title: lstData[i].find(x => x.columnAlias == "productName")?.value,
                vendor: lstData[i].find(x => x.columnAlias == "vendor")?.value,
                id: Number(lstId[i]),
                body_html: "",
                status: lstData[i].find(x => x.columnAlias == "status")?.value,
                tags: lstData[i].find(x => x.columnAlias == "tags")?.value,
                variants: [],
                image: {
                    src: "",
                    alt: ""
                },
                product_type: "",
                published_scope: "",
                handle: "",
            }

            productList.push(dto);
        }

        return productList;
    }

    _onSubmit() {
        if (this._numberOfSelection == 0){
            this._rejectModal();
        } else {
            if (!this.checkNumberOfSelection()) {
                this._showError("Please select the amount of items that has been configured in the setting.");
            } else {
                this.value = { productList: this._selectedProducts };
                this._submitModal();
            }
        }
    }

    private checkNumberOfSelection(){
        return this._numberOfSelection >= this._minimumItems && this._numberOfSelection <= this._maximumItems;
    }

    #onPageChange(event: UUIPaginationEvent) {
        const forward = event.target?.current > this._currentPageNumber;

        const currentPageNumber = forward ? this._currentPageNumber + 1 : this._currentPageNumber - 1

        this.#paginationManager.setCurrentPageNumber(currentPageNumber);

        this._currentPageNumber = currentPageNumber;
        this.#loadProducts(forward ? this._nextPageInfo : this._previousPageInfo);
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
                    ${this._loading ? html`<div class="center loader"><uui-loader></uui-loader></div>` : ""}
                    <umb-table 
                        .config=${this._tableConfig} 
                        .columns=${this._tableColumns} 
                        .items=${this._tableItems}
                        .selection=${this._selection}
                        @selected="${this.#onSelected}"
                        @deselected="${this.#onDeselected}">
                    </umb-table>
                    ${this.#renderPagination()}
                </uui-box>

                ${this._maximumItems > 0
                    ? html`
                        <div class="maximum-selection">
                            <span>
                                Add up to ${this._maximumItems} items(s)
                            </span>
                        </div>
                    `
                    : nothing
                }
                <uui-button look="primary"  slot="actions" label="Submit" @click=${this._onSubmit}></uui-button>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    #renderPagination() {
        return html`
            ${this._totalPages > 1
             ? html`
                <div class="shopify-pagination">
                    <uui-pagination
					    class="pagination"
					    .current=${this._currentPageNumber}
					    .total=${this._totalPages}
					    @change=${this.#onPageChange}></uui-pagination>
                </div>
             `
             : nothing}
        `;
    }

    static styles = [css`
        .loader {
            display: flex;
            justify-content: center;
        }
        .maximum-selection{
            margin-top: 10px;
            font-weight: bold;
        }
        .shopify-pagination {
            width: 50%;
            margin-top: 10px;
            margin-left: auto;
            margin-right: auto;
        }
    `];
}