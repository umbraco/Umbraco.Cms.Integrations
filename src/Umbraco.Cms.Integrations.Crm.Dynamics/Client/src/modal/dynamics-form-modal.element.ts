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

        } else {
            if (!data.isAuthorized) {
                this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.")
            } else {

                await this.#getForms();
            }
        }
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
            
            ${this._loading ? 
                html`<div class="center loader"><uui-loader></uui-loader></div>` : 
                html`
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
                        <dynamics-authorization></dynamics-authorization>
                    </uui-box>
                `}

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }
}

