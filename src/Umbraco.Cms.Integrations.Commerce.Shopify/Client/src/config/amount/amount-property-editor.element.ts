import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN } from "../../context/shopify.context";

const elementName = "shopify-amount";

@customElement(elementName)
export class ShopifyAmountElement extends UmbElementMixin(LitElement){
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;
    @property()
    minValue: number = 0;
    maxValue: number = 0;

    render() {
        return html`
            <div>
                <uui-input .value=${this.minValue}></uui-input>
                <span>-</span>
                <uui-input placeholder="∞" .value=${this.maxValue}></uui-input>
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