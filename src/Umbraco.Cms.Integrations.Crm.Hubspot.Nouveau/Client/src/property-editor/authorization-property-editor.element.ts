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
import {
    ConfigDescription,
    HubspotServiceStatus
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
        const { data } = await this.#hubspotFormsContext!.checkApiConfiguration();
        if (!data) return;

        this._serviceStatus.isValid = data!.isValid;
        this._serviceStatus.type = data!.type?.value!;
        this._serviceStatus.description = this.#getDescription(this._serviceStatus.type);
        this._serviceStatus.useOAuth = data!.isValid && data!.type?.value! == "OAuth";

        if (this._serviceStatus.useOAuth) {
            await this.#validateOAuthSetup();
        }

        if (!data!.isValid) {
            const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
            notificationContext?.peek("danger", {
                data: { message: "Invalid setup. Please review the API/OAuth settings." },
            });
        }
    }

    async #validateOAuthSetup() {
        const { data } = await this.#hubspotFormsContext.validateAccessToken();
        if (data) {
            console.log(data);
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

    render() {
        return html`
            <div>
                <p>${this._serviceStatus.isValid}</p>
            </div>
            ${this._serviceStatus.useOAuth
                ? html`
                    <div>
                        <uui-button look="primary" label="Connect">Connect</uui-button>
                        <uui-button look="primary" color="danger" label="Revoke">Revoke</uui-button>
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

export type MyType = {
    isValid: boolean;
    name: string;
}