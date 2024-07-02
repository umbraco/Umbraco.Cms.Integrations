import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    LitElement,
    customElement,
    html,
    property,
    state
} from "@umbraco-cms/backoffice/external/lit";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import { HUBSPOT_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/hubspot-forms/context";
import { OAuthRequestDtoModel } from "@umbraco-integrations/hubspot-forms/generated";
import {
    ConfigDescription,
    type HubspotOAuthSetup,
    type HubspotServiceStatus
} from "../models/hubspot-service.model.js";

const elementName = "hubspot-authorization";

@customElement(elementName)
export class HubspotAuthorizationElement extends UmbElementMixin(LitElement) {

    #hubspotFormsContext!: typeof HUBSPOT_FORMS_CONTEXT_TOKEN.TYPE;

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

    constructor() {
        super();

        this.consumeContext(HUBSPOT_FORMS_CONTEXT_TOKEN, (context) => {
            this.#hubspotFormsContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        await this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        const { data } = await this.#hubspotFormsContext?.checkApiConfiguration();
        if (!data) return;

        this._serviceStatus = {
            isValid: data!.isValid,
            type: data!.type?.value!,
            description: this.#getDescription(this._serviceStatus.type),
            useOAuth: data!.isValid && data!.type?.value! == "OAuth"
        }

        if (this._serviceStatus.useOAuth) {
            await this.#validateOAuthSetup();
        }

        if (!data!.isValid) {
            this._showError("Invalid setup. Please review the API/OAuth settings.");
        }
    }

    async #validateOAuthSetup() {
        const { data } = await this.#hubspotFormsContext?.validateAccessToken();
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
            case "api": return ConfigDescription.api; break;
            case "oauth": return ConfigDescription.oauth; break;
            case "oauthConnected": return ConfigDescription.oauthConnected; break;
            default: return ConfigDescription.none; break;
        }
    }

    async #onConnect() {
        window.addEventListener("message", async (event: MessageEvent) => {
            if (event.data.type === "hubspot:oauth:success") {

                const oauthRequestDtoModel: OAuthRequestDtoModel = {
                    code: event.data.code
                };

                const { data } = await this.#hubspotFormsContext?.getAccessToken(oauthRequestDtoModel);
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

        const { data } = await this.#hubspotFormsContext?.getAuthorizationUrl();
        if (!data) return;

        window.open(data, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    async #onRevoke() {
        await this.#hubspotFormsContext.revokeAccessToken();

        this._oauthSetup = {
            isConnected: false
        };
        this._serviceStatus.description = ConfigDescription.none;
        this._showSuccess("OAuth connection revoked.");
    }

    private async _showSuccess(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("positive", {
            data: { message: message },
        });
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message },
        });
    }

    render() {
        return html`
            <div>
                <p>${this._serviceStatus.description}</p>
            </div>
            ${this._serviceStatus.useOAuth
                ? html`
                    <div>
                        <uui-button look="primary" 
                                    label="Connect"
                                    ?disabled=${this._oauthSetup.isConnected}
                                    @click=${this.#onConnect}>
                                    Connect
                        </uui-button>
                        <uui-button look="primary"
                                    color="danger"
                                    label="Revoke"
                                    ?disabled=${!this._oauthSetup.isConnected}
                                    @click=${this.#onRevoke}>
                                    Revoke
                        </uui-button>
                    </div>
                `
                : ""
            }
        `;
    }
}

export default HubspotAuthorizationElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: HubspotAuthorizationElement;
    }
}