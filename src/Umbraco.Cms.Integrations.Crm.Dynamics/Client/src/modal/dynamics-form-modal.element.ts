import { css, customElement, html, nothing, property, repeat, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { DynamicsFormPickerModalData, DynamicsFormPickerModalValue } from "./dynamics.modal-token";
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { DynamicsModuleModel, FormDtoModel, OAuthConfigurationDtoModel } from "@umbraco-integrations/dynamics/generated";
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationColor } from "@umbraco-cms/backoffice/notification";
import { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";
import {
    type UmbPropertyEditorConfigCollection,
    UmbPropertyValueChangeEvent
} from "@umbraco-cms/backoffice/property-editor";
import * as dynamicsModuleHelper from "../helpers/dynamic-module.helper";

const elementName = "dynamics-forms-modal";

@customElement(elementName)
export default class DynamicsFormModalElement extends UmbModalBaseElement<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>{
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    #settingsModel?: OAuthConfigurationDtoModel;

    @state()
    private _loading = false;
    @state()
    private _forms: Array<FormDtoModel> = [];
    @state()
    private _filteredForms: Array<FormDtoModel> = [];
    @state()
    private _selectedForm: FormDtoModel = {
        id: "",
        module: DynamicsModuleModel.BOTH,
        name: "",
        rawHtml: "",
        standaloneHtml: ""
    };
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

    async #checkOAuthConfiguration() {
        if (!this.#settingsModel) return;

        if (!this.#settingsModel.isAuthorized) {
            this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.");
        } else {
            await this.#getForms();
        }
    }

    async #getForms() {
        this._loading = true;
        const { data } = await this.#dynamicsContext.getForms(this.data?.module!);
        if (!data) return;

        this._forms = data;
        this._filteredForms = data;
        this._loading = false;
    }

    async _onSelect(form: FormDtoModel) {
        this._selectedForm = form;
    }

    async _onSubmit() {
        if (this.renderWithIFrame && dynamicsModuleHelper.parseModule(this._selectedForm.module.toString()) == DynamicsModuleModel.OUTBOUND) {
            var { data } = await this.#dynamicsContext.getEmbedCode(this._selectedForm.id);
            if (!data || data.result.length == 0) {
                this._showError("Unable to embed selected form. Please check if it is live.");
                return false;
            }
        }

        this.value = { selectedForm: this._selectedForm, iframeEmbedded: this.renderWithIFrame };
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

    onMessageOnSubmitIsHtmlChange() {
        this.renderWithIFrame = !this.renderWithIFrame;
        this.toggleLabel = !this.renderWithIFrame ? "Render with Script" : "Render with iFrame";
    }

    #handleFilterInput(event: UUIInputEvent) {
        let query = (event.target.value as string) || '';
        query = query.toLowerCase();

        const result = !query
            ? this._forms
            : this._forms.filter((form) => form.name?.toLowerCase().includes(query));

        this._filteredForms = result;
    }
    private _renderFilter() {
        return html` <uui-input
			type="search"
			id="filter"
			@input="${this.#handleFilterInput}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
    }

    render() {
        return html`
            <umb-body-layout>
            ${this._loading ?
                html`<div class="center loader"><uui-loader></uui-loader></div>` :
                html`
                    <uui-box headline=${this.data!.headline}>
                            ${this._renderFilter()}
                            ${this._filteredForms.length > 0 ?
                                html`
                                            ${repeat(this._filteredForms, (form) => html`
                                                <uui-ref-node-form
                                                    selectable
                                                    ?selected=${this._selectedForm.id == form.id}
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
                        <dynamics-authorization></dynamics-authorization>
                    </uui-box>
                `}

                <uui-button look="primary" slot="actions" label="Submit" @click=${this._onSubmit}></uui-button>
                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
            #filter {
                width: 100%;
                margin-bottom: var(--uui-size-3);
            }

            uui-icon {
                margin: auto;
                margin-left: var(--uui-size-2);
            }
        `];
}

