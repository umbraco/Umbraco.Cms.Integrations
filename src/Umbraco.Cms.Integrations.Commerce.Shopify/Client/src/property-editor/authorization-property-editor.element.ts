import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context";

const elementName = "shopify-authorization";

@customElement(elementName)
export class ShopifyAuthorizationElement extends UmbElementMixin(LitElement){
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;

    render() {
        return html`
            <div><span>Authentication</span></div>
        `;
    }
}