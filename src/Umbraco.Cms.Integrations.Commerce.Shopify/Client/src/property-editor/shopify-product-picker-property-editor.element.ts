import { customElement, html, css, property, state, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";
import { SHOPIFY_MODAL_TOKEN } from "../modal/shopify.modal-token";
import { ConfigDescription, type ShopifyServiceStatus } from "../models/shopify-service.model";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context";
import type { EditorSettingsModel, ProductDtoModel } from "@umbraco-integrations/shopify/generated";
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { ShopifyProductPickerConfiguration } from "../types/types";

const elementName = "shopify-product-picker";

@customElement(elementName)
export class ShopifyProductPickerPropertyEditor extends UmbLitElement implements UmbPropertyEditorUiElement {
    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;
    #settingsModel?: EditorSettingsModel;

    @state()
	private _config?: ShopifyProductPickerConfiguration;

    @property({ attribute: false })
	public set config(config: UmbPropertyEditorConfigCollection) {
		this._config = this.#mapDataTypeConfigToCollectionConfig(config);
	}

    #mapDataTypeConfigToCollectionConfig(
		config: UmbPropertyEditorConfigCollection | undefined,
	) : ShopifyProductPickerConfiguration {
		return {
			minItems: config?.getValueByAlias('minItems'),
            maxItems: config?.getValueByAlias('maxItems')
		};
	}

    @property({ type: String })
    public value = "";

    @state()
    private products: Array<ProductDtoModel> = [];

    @state()
    private _serviceStatus: ShopifyServiceStatus = {
        isValid: false,
        type: "",
        description: "",
        useOAuth: false
    };

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
        this.consumeContext(SHOPIFY_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.observe(context.settingsModel, (settingsModel) => {
                this.#settingsModel = settingsModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        if (!this.#settingsModel) return;

        this._serviceStatus = {
            isValid: this.#settingsModel.isValid,
            type: this.#settingsModel.type.value,
            description: "",
            useOAuth: this.#settingsModel.isValid && this.#settingsModel.type.value === "OAuth"
        }

        if (!this._serviceStatus.isValid) {
            this._showError(ConfigDescription.none);
        }

        if (this.value == null || this.value.length == 0) return;

        const dto: Array<ProductDtoModel> = JSON.parse(JSON.stringify(this.value));
        this.products = dto;
    }

    private async _openModal() {
        const pickerContext = this.#modalManagerContext?.open(this, SHOPIFY_MODAL_TOKEN, {
            data: {
                headline: "Shopify Products",
                selectedItemIdList: this.products.map(p => p.id.toString()),
                config: this._config
            },
        });

        const data = await pickerContext?.onSubmit();
        if (!data) return;

        this.value = JSON.stringify(data.productList);
        this.products = JSON.parse(this.value);
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message }
        });
    }

    deleteForm(id: number){
        var index = this.products.map(p => p.id).indexOf(id);
        this.products.splice(index, 1);
        this.value = JSON.stringify(this.products);
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    render() {
        return html`
            ${this._serviceStatus.isValid 
                ? html`
                    <div>
                        <uui-button
                            class="add-button"
                            @click=${this._openModal}
                            label=${this.localize.term('general_add')}
                            look="placeholder"></uui-button>
                    </div>
                    <div>
                        ${repeat(this.products, (product) => 
                            html`
                                <uui-ref-node-form name=${product.title} detail=${product.vendor}>
                                    <uui-action-bar slot="actions">
                                        <uui-button label="Remove" @click=${() => this.deleteForm(product.id)}>Remove</uui-button>
                                    </uui-action-bar>
                                </uui-ref-node-form>
                            `
                        )}    
                    </div>
                ` 
                : html`
                    <span></span>
                `}
            `;
    }

    static styles = [
        css`
            .add-button {
                width: 100%;
            }
        `];
}


export default ShopifyProductPickerPropertyEditor;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: ShopifyProductPickerPropertyEditor;
    }
}