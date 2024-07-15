import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context.js";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { html, css, state } from "@umbraco-cms/backoffice/external/lit";
import type { ProductDtoModel } from "../../generated";
import type { ShopifyProductPickerModalData, ShopifyProductPickerModalValue } from "./shopify.modal-token.js";
import type { ShopifyServiceStatus } from "../models/shopify-service.model.js";

const elementName = "shopify-products-modal";
export default class ShopifyProductsModalElement extends UmbModalBaseElement<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>{
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;

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
        const { data } = await this.#shopifyContext.getList();
        if (!data) return;

        this._products = data.products ?? [];
        this._loading = false;

        if (!data.isValid || data.isExpired) {
            this._showError(data.error!);
        }
    }

    private _onSelect(products: Array<ProductDtoModel>) {
        this.value = { products };
        this._submitModal();
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
                <uui-box headline="Shopify Products">
                    ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${this._products.map((product) => {
                        return html`
                            <umb-table></umb-table>
                        `;
                    })}
                </uui-box>
                <span>Add up to x items(s)</span>

                <uui-button slot="actions" label="Submit"></uui-button>
                <uui-button slot="actions" label="Close"></uui-button>
            </umb-body-layout>
        `;
    }
}