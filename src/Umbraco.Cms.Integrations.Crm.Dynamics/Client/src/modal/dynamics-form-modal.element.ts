import { customElement, html, repeat, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { DynamicsFormPickerModalData, DynamicsFormPickerModalValue } from "./dynamics.modal-token";
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { FormDtoModel, OAuthConfigurationDtoModel, OAuthRequestDtoModel } from "@umbraco-integrations/dynamics/generated";
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationColor } from "@umbraco-cms/backoffice/notification";
import { DynamicsOAuthSetup } from "../models/dynamics-service.model";

const elementName = "dynamics-forms-modal";

@customElement(elementName)
export default class DynamicsFormModalElement extends UmbModalBaseElement<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>{
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    
    @state()
    private _loading = false;
    @state()
    private _forms: Array<FormDtoModel> = [];
    @state()
    private _oAuthConfig: OAuthConfigurationDtoModel | undefined;
    @state()
    private _oauthSetup: DynamicsOAuthSetup = {
        isConnected: false,
        isAccessTokenExpired: false,
        isAccessTokenValid: false
    };

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
        const {data} = await this.#dynamicsContext.checkOauthConfiguration();
        if(!data) return;
        if(!data.isAuthorized) this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.");

        this._oAuthConfig = data;

        await this.#getForms();
    }

    async #getForms(){
        this._loading = true;
        const { data } = await this.#dynamicsContext.getForms("Both");
        if(!data) return;

        this._forms = data;
        this._loading = false;
    }

    _onSelect(form: FormDtoModel){
        this.value = { selectedForm: form };
        this._submitModal();
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
            <umb-body-layout>
            
            ${this._loading ? html`<div class="center loader"><uui-loader></uui-loader></div>` : ""}
                
            <uui-box headline=${this.data!.headline}>
                    <div slot="header">Select a form</div>

                    ${repeat(this._forms, (form) => html`
                        <uui-ref-node-form
                            selectable
                            name=${form.name ?? ""}
                            @selected=${() => this._onSelect(form)}>
                        </uui-ref-node-form>
                    `)}
                </uui-box>
                <br />
                <uui-box headline="Dynamics - OAuth Status">
                    <div slot="header">Please connect with your Microsoft account.</div>
                    <div>
                        <p><b>Connected</b>: ${this._oAuthConfig?.fullName}</p>
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
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }
}

