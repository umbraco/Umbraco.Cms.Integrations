import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html, property, state, when } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT, type UmbNotificationColor} from "@umbraco-cms/backoffice/notification";
import { ConfigDescription, type ShopifyOAuthSetup, type ShopifyServiceStatus } from "../models/shopify-service.model";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context";
import type { OAuthRequestDtoModel } from "../generated";

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