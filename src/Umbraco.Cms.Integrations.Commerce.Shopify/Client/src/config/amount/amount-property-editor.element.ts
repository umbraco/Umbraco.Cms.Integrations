import { customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyValueChangeEvent, UmbPropertyEditorUiElement, type UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

const elementName = "shopify-amount";

@customElement(elementName)
export class ShopifyAmountElement extends UmbLitElement implements UmbPropertyEditorUiElement{
    @property({ type: Number })
	min?: number;

    @property({ type: Number })
	max?: number;

    public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.min = this.#parseInt(config.getValueByAlias('amountMin'));
		this.max = this.#parseInt(config.getValueByAlias('amountMax'));
	}

    #parseInt(input: unknown): number | undefined {
		const num = Number(input);
		return Number.isNaN(num) ? undefined : num;
	}

    #onMinInput(e: InputEvent & { target: HTMLInputElement }) {
		this.min = this.#parseInt(e.target.value);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

    #onMaxInput(e: InputEvent & { target: HTMLInputElement }) {
		this.max = this.#parseInt(e.target.value);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}
    
    override render() {
        return html`
            <div>
                <uui-input
				    type="number"
				    .value=${this.min}
				    @input=${this.#onMinInput}></uui-input>
			    </uui-input>
                <span>-</span>
                <uui-input
				    type="number"
				    .value=${this.max}
				    @input=${this.#onMaxInput}></uui-input>
			    </uui-input>
            </div>
        `;
    }
}

export default ShopifyAmountElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: ShopifyAmountElement;
	}
}