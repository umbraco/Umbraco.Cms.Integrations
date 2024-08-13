import { customElement, html, repeat, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { DynamicsFormPickerModalData, DynamicsFormPickerModalValue } from "./dynamics.modal-token";
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { FormDtoModel, OAuthConfigurationDtoModel } from "@umbraco-integrations/dynamics/generated";
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationColor } from "@umbraco-cms/backoffice/notification";
import { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";
import { UmbPropertyValueChangeEvent } from "@umbraco-cms/backoffice/property-editor";

const elementName = "dynamics-forms-modal";
enum testEnum {
    OUTBOUND = "1",
    REAL_TIME = "2"
}

@customElement(elementName)
export default class DynamicsFormModalElement extends UmbModalBaseElement<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>{
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    #settingsModel?: OAuthConfigurationDtoModel;

    @state()
    private _loading = false;
    @state()
    private _forms: Array<FormDtoModel> = [];
    @state()
    private _filterForms: Array<FormDtoModel> = [];
    @state()
    private renderWithIFrame: boolean = false;
    @state()
    private toggleLabel: string = "Render with Script";
    

    constructor() {
        super();

        this.consumeContext(DYNAMICS_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#dynamicsContext = context;
            this.observe(context.settingsModel, (settingsModel) => {
                this.#settingsModel = settingsModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        await this.#checkOAuthConfiguration();
    }

    async #checkOAuthConfiguration(){
        if (!this.#settingsModel) return;

        if (!this.#settingsModel.isAuthorized) {
            this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.");
        } else {
            await this.#getForms();
        }
    }

    async #getForms(){
        this._loading = true;
        const { data } = await this.#dynamicsContext.getForms("Both");
        if(!data) return;

        this._forms = data;
        this._filterForms = data;
        this._loading = false;
    }

    async _onSelect(form: FormDtoModel){
        var result = await this.checkEmbed(form);
        if (result){
            this.value = { selectedForm: form };
            this._submitModal();
        }
    }

    async checkEmbed(form: FormDtoModel){
        if(this.renderWithIFrame && form.module.toString() === testEnum.REAL_TIME){
            var { data } = await this.#dynamicsContext.getEmbedCode(form.id);
            if (!data || data.result.length == 0){
                this._showError("Unable to embed selected form. Please check if it is live.");
                return false;
            }
        }

        return true;
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

    onMessageOnSubmitIsHtmlChange(){
        this.renderWithIFrame = !this.renderWithIFrame;
        this.toggleLabel = !this.renderWithIFrame ? "Render with Script" : "Render with iFrame";
    }

    onSearchChange(e: UUIInputEvent){
        var searchText = e.target?.value as string;
        this._filterForms = this._forms.filter(f => f.name.toLowerCase().includes(searchText.toLowerCase()));
        this.dispatchEvent(new UmbPropertyValueChangeEvent());
    }

    render(){
        return html`
            <umb-body-layout>
            ${this._loading ? 
                html`<div class="center loader"><uui-loader></uui-loader></div>` : 
                html`
                    <uui-box headline=${this.data!.headline}>
                            <div slot="header">Select a form</div>

                            <uui-input placeholder="Type to search..." @change=${(e: UUIInputEvent) => this.onSearchChange(e)}>
                                <div slot="append" style="background:#f3f3f3; padding-left:var(--uui-size-2, 6px)">
                                    <uui-icon-registry-essential>
                                        <uui-icon name="search"></uui-icon>
                                    </uui-icon-registry-essential>
                                </div>
                            </uui-input>

                            ${this._filterForms.length > 0 ? 
                                html`
                                    ${repeat(this._filterForms, (form) => html`
                                        <uui-ref-node-form
                                            selectable
                                            name=${form.name ?? ""}
                                            @selected=${() => this._onSelect(form)}>
                                        </uui-ref-node-form>
                                    `)}

                                    <uui-toggle
                                        ?checked=${this.renderWithIFrame}
                                        .label=${this.toggleLabel}
                                        @change=${this.onMessageOnSubmitIsHtmlChange}></uui-toggle>
                                ` : 
                                html``}
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

