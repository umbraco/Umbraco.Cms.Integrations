import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    LitElement,
    css,
    customElement,
    html,
    property,
    query,
    state,
    when
} from "@umbraco-cms/backoffice/external/lit";
import {
    UMB_NOTIFICATION_CONTEXT,
    type UmbNotificationColor,
} from "@umbraco-cms/backoffice/notification";
import {
    ConfigDescription,
    type HubspotOAuthSetup,
    type HubspotServiceStatus
} from "../models/hubspot-service.model.js";
import { HUBSPOT_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/hubspot-forms/context";
import type { HubspotFormPickerSettingsModelReadable, OAuthRequestDtoModel } from "@umbraco-integrations/hubspot-forms/generated";

const elementName = "hubspot-authorization";

@customElement(elementName)
export class HubspotAuthorizationElement extends UmbElementMixin(LitElement) {

    #hubspotFormsContext!: typeof HUBSPOT_FORMS_CONTEXT_TOKEN.TYPE;
    #settingsModel?: HubspotFormPickerSettingsModelReadable;

    @state()
    private _serviceStatus: HubspotServiceStatus = {
        isValid: false,
        type: "",
        description: "",
        useOAuth: false
    };

    @state()
    private _oauthSetup: HubspotOAuthSetup = {
        isConnected: false,
        isAccessTokenExpired: false,
        isAccessTokenValid: false
    };

    @property({ type: String })
    public value = "";

    @state()
    showAuthTokenComponent: boolean = false;

    @query('#auth-code-input')
    private _authCodeInput!: HTMLInputElement;

    constructor() {
        super();
        this.consumeContext(HUBSPOT_FORMS_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#hubspotFormsContext = context;
            this.observe(context.settingsModel, (settingsModel) => {
                this.#settingsModel = settingsModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        await this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {

        if (!this.#settingsModel) return;

        this._serviceStatus = {
            isValid: this.#settingsModel.isValid,
            type: this.#settingsModel.type?.value!,
            description: this.#getDescription(this._serviceStatus.type),
            useOAuth: this.#settingsModel.isValid && this.#settingsModel.type?.value === "OAuth"
        }

        if (this._serviceStatus.useOAuth) {
            await this.#validateOAuthSetup();
        }

        if (!this.#settingsModel.isValid) {
            this._showError("Invalid setup. Please review the API/OAuth settings.");
        }
    }

    async #validateOAuthSetup() {
        const { data } = await this.#hubspotFormsContext.validateAccessToken();
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
                await this.#hubspotFormsContext.refreshAccessToken();
            }
        }
    }

    #getDescription(type: string): string {
        switch (type) {
            case "api": return ConfigDescription.api;
            case "oauth": return ConfigDescription.oauth;
            case "oauthConnected": return ConfigDescription.oauthConnected;
            default: return ConfigDescription.none;
        }
    }

    async #onConnect() {
        const { data } = await this.#hubspotFormsContext.getAuthorizationUrl();
        if (!data) return;

        var authCodeElement = document.getElementById("authToken");
        if (authCodeElement) {
            authCodeElement.style.display = "none";
        }

        const authWin = window.open(data, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");

        // check in 7 seconds for authorization window closed
        setTimeout(() => {
            if (!authWin?.closed) {
                this.showAuthTokenComponent = true;
            }
        }, 7000);

        window.addEventListener("message", async (event: MessageEvent) => {

            if (event.data.type === "hubspot:oauth:success") {
                const oauthRequestDtoModel: OAuthRequestDtoModel = {
                    code: event.data.code
                };

                await this.getAccessToken(oauthRequestDtoModel);

            }
        }, false);
    }

    async #onRevoke() {
        await this.#hubspotFormsContext.revokeAccessToken();

        this._oauthSetup = {
            isConnected: false
        };
        this._serviceStatus.description = ConfigDescription.none;
        this._showSuccess("OAuth connection revoked.");

        this.dispatchEvent(new CustomEvent("revoke"));
    }

    async #onAuthorize() {
        if (this._authCodeInput.value.length == 0) {
            this._showError("Incorrect authorization code.");
            return;
        }
        const oauthRequestDtoModel: OAuthRequestDtoModel = {
            code: this._authCodeInput.value
        };
        await this.getAccessToken(oauthRequestDtoModel);
    }

    async getAccessToken(oauthRequestDtoModel: OAuthRequestDtoModel) {
        const { data } = await this.#hubspotFormsContext.getAccessToken(oauthRequestDtoModel);
        if (!data) return;

        this.showAuthTokenComponent = false;

        if (data.startsWith("Error:")) {
            this._showError(data);
        } else {
            this._oauthSetup = {
                isConnected: true
            };
            this._serviceStatus.description = ConfigDescription.oauthConnected;
            this._showSuccess("OAuth Connected");

            this.dispatchEvent(new CustomEvent("connect"));
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

    render() {
        return html`
            <p>${this._serviceStatus.description}</p>
            ${when(this._serviceStatus.useOAuth, () =>
                html`
                    <div id="authConnect">
                        <uui-button look="primary" 
                                    label="Connect"
                                    ?disabled=${this._oauthSetup.isConnected}
                                    @click=${this.#onConnect}></uui-button>
                        <uui-button look="primary"
                                    color="danger"
                                    label="Revoke"
                                    ?disabled=${!this._oauthSetup.isConnected}
                                    @click=${this.#onRevoke}></uui-button>
                    </div>
                    ${when(this.showAuthTokenComponent, () => html`
                        <div id="authToken">
                            <uui-input id="auth-code-input" placeholder="Authorization code"></uui-input>
                            <uui-button look="primary"
                                        label="Authorize"
                                        @click=${this.#onAuthorize}></uui-button>
                        </div>
                    `)}
                `
            )}
        `;
    }

    static styles = [
        css`
            #authToken { 
                margin-top: 20px; 
            }
            #authToken uui-input {
                width: 50%;
                vertical-align: middle;
            }
        `];
}

export default HubspotAuthorizationElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: HubspotAuthorizationElement;
    }
}
