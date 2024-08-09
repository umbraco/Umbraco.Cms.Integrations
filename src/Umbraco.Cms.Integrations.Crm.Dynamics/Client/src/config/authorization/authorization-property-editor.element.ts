import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html, property, state, when } from "@umbraco-cms/backoffice/external/lit";
import { DynamicsOAuthSetup } from "../../models/dynamics-service.model";
import { DYNAMICS_CONTEXT_TOKEN } from "../../context/dynamics.context";
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationColor } from "@umbraco-cms/backoffice/notification";
import { OAuthConfigurationDtoModel, OAuthRequestDtoModel } from "@umbraco-integrations/dynamics/generated";

const elementName = "dynamics-authorization";

@customElement(elementName)
export class DynamicsAuthorizationElement extends UmbElementMixin(LitElement){
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;

    @state()
    private _oauthSetup: DynamicsOAuthSetup = {
        isConnected: false,
        isAccessTokenExpired: false,
        isAccessTokenValid: false
    };

    @state()
    private _oAuthConfig: OAuthConfigurationDtoModel | undefined;

    constructor() {
        super();

        this.consumeContext(DYNAMICS_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#dynamicsContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        
        await this.#checkOAuthConfiguration();
    }

    async #checkOAuthConfiguration(){
        const { data } = await this.#dynamicsContext.checkOauthConfiguration();
        if (!data) {
            this._oauthSetup = {
                isConnected: false,
                isAccessTokenExpired: true,
                isAccessTokenValid: false
            }
        } else {
            if (!data.isAuthorized) {
                this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.")
            } else {
                this._oauthSetup = {
                    isConnected: true,
                    isAccessTokenExpired: false,
                    isAccessTokenValid: true
                }

                this._oAuthConfig = data;
            }
        }
    }

    async #connectButtonClick(){
        window.addEventListener("message", async (event: MessageEvent) => {
            if (event.data.type === "dynamics:oauth:success") {

                const oauthRequestDtoModel: OAuthRequestDtoModel = {
                    code: event.data.code
                };

                const { data } = await this.#dynamicsContext.getAccessToken(oauthRequestDtoModel);
                if (!data) return;

                if (data.startsWith("Error:")) {
                    this._showError(data);
                } else {
                    this._oauthSetup = {
                        isConnected: true
                    };
                    this._showSuccess("OAuth Connected");

                }

            }
        }, false);

        const { data } = await this.#dynamicsContext.getAuthorizationUrl();
        if (!data) return;

        window.open(data, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    async #revokeButtonClick(){
        await this.#dynamicsContext.revokeAccessToken();

        this._oauthSetup = {
            isConnected: false
        };
        this._showSuccess("OAuth connection revoked.");

        this.dispatchEvent(new CustomEvent("revoke"));
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

    render(){
        return html`
            <div>
                ${this._oauthSetup.isConnected ? 
                    html`
                        <span>
                            <b>Connected</b>: ${this._oAuthConfig?.fullName}
                        </span>
                    ` : 
                    html`
                        <span>
                            <b>Disconnected</b>
                        </span>
                    `}
                
            </div>
            <div>
                <uui-button 
                    look="primary" 
                    label="Connect"
                    ?disabled=${this._oauthSetup?.isConnected}
                    @click=${this.#connectButtonClick}></uui-button>
                <uui-button 
                    color="danger" 
                    look="secondary" 
                    label="Revoke"
                    ?disabled=${!this._oauthSetup?.isConnected}
                    @click=${this.#revokeButtonClick}></uui-button>
            </div>
        `;
    }
}

export default DynamicsAuthorizationElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: DynamicsAuthorizationElement;
	}
}