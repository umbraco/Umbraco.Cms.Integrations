import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN } from "../../context/shopify.context";

const elementName = "shopify-amount";

@customElement(elementName)
export class ShopifyAmountElement extends UmbElementMixin(LitElement){
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;

    render() {
        return html`
            <div>
                <uui-input></uui-input>
                <span>-</span>
                <uui-input placeholder="∞"></uui-input>
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