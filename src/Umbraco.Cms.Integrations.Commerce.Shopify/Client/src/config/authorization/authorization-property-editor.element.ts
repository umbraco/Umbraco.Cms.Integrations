import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html, property, state, when } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN } from "../../context/shopify.context";
import { ConfigDescription, ShopifyOAuthSetup, type ShopifyServiceStatus } from "../../models/shopify-service.model";
import { OAuthRequestDtoModel } from "../../../generated";
import { UMB_NOTIFICATION_CONTEXT, type UmbNotificationColor } from "@umbraco-cms/backoffice/notification";

const elementName = "shopify-authorization";

@customElement(elementName)
export class ShopifyAuthorizationElement extends UmbElementMixin(LitElement){
    #shopifyContext!: typeof SHOPIFY_CONTEXT_TOKEN.TYPE;

    @state()
    private _serviceStatus: ShopifyServiceStatus = {
        isValid: false,
        type: "",
        description: "",
        useOAuth: false
    };

    @state()
    private _oauthSetup: ShopifyOAuthSetup = {
        isConnected: false,
        isAccessTokenExpired: false,
        isAccessTokenValid: false
    };

    @property({ type: String })
    public value = "";

    constructor() {
        super();

        this.consumeContext(SHOPIFY_CONTEXT_TOKEN, (context) => {
            this.#shopifyContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        await this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        const { data } = await this.#shopifyContext.checkConfiguration();
        if (!data || !data.type?.value) return;

        this._serviceStatus = {
            isValid: data.isValid,
            type: data.type.value,
            description: this.#getDescription(data.type.value),
            useOAuth: data.isValid && data.type.value === "OAuth"
        }

        if (this._serviceStatus.useOAuth) {
            await this.#validateOAuthSetup();
        }

        if (!data!.isValid) {
            this._showError("Invalid setup. Please review the API/OAuth settings.");
        }
    }

    async #validateOAuthSetup() {
        const { data } = await this.#shopifyContext.validateAccessToken();
        if (data) {
            this._oauthSetup = {
                isConnected: data.isValid,
                isAccessTokenExpired: data.isExpired,
                isAccessTokenValid: data.isValid
            }

            if (this._oauthSetup.isConnected && this._oauthSetup.isAccessTokenValid) {
                this._serviceStatus.description = ConfigDescription.oauthConnected;
            }

            if (this._oauthSetup.isAccessTokenExpired) {
                await this.#shopifyContext.refreshAccessToken();
            }
        }
    }

    #getDescription(type: string) {
        switch (type) {
            case "API": return ConfigDescription.api;
            case "OAuth": return ConfigDescription.oauth;
            case "OAuthConnected": return ConfigDescription.oauthConnected;
            default: return ConfigDescription.none;
        }
    }

    private async _showSuccess(message: string) {
        await this._showMessage(message, "positive");
    }

    private async _showError(message: string) {
        await this._showMessage(message, "danger");
    }

    private async _showMessage(message: string, color: UmbNotificationColor) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek(color, {
            data: { message },
        });
    }

    async #connectButtonClick(){
        window.addEventListener("message", async (event: MessageEvent) => {
            if (event.data.type === "shopify:oauth:success") {

                const oauthRequestDtoModel: OAuthRequestDtoModel = {
                    code: event.data.code
                };

                const { data } = await this.#shopifyContext.getAccessToken(oauthRequestDtoModel);
                if (!data) return;

                if (data.startsWith("Error:")) {
                    this._showError(data);
                } else {
                    this._oauthSetup = {
                        isConnected: true
                    };
                    this._serviceStatus.description = ConfigDescription.oauthConnected;
                    this._showSuccess("OAuth Connected");

                }

            }
        }, false);

        const { data } = await this.#shopifyContext.getAuthorizationUrl();
        if (!data) return;

        window.open(data, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    async #revokeButtonClick(){
        await this.#shopifyContext.revokeAccessToken();

        this._oauthSetup = {
            isConnected: false
        };
        this._serviceStatus.description = ConfigDescription.none;
        this._showSuccess("OAuth connection revoked.");
    }

    render() {
        return html`
            <div>
                <p>${this._serviceStatus.description}</p>
            </div>
            ${when(this._serviceStatus.useOAuth, () => 
                html`
                <div>
                    <uui-button 
                        look="primary" 
                        label="Connect" 
                        ?disabled=${this._oauthSetup.isConnected} 
                        @click=${this.#connectButtonClick}></uui-button>
                    <uui-button 
                        color="danger" 
                        look="secondary" 
                        label="Revoke" 
                        ?disabled=${!this._oauthSetup.isConnected} 
                        @click=${this.#revokeButtonClick}></uui-button>
                </div>
                `)}
            
        `;
    }
}

export default ShopifyAuthorizationElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: ShopifyAuthorizationElement;
	}
}