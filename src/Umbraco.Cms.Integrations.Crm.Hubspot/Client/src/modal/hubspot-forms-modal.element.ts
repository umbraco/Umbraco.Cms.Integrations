import { html, css, state, customElement } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";
import type { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";
import type { HubspotServiceStatus } from "../models/hubspot-service.model.js";
import type { HubspotFormPickerModalData, HubspotFormPickerModalValue } from "./hubspot.modal-token.js";
import type { HubspotFormDtoModel, HubspotFormPickerSettingsModelReadable } from "@umbraco-integrations/hubspot-forms/generated";
import { HUBSPOT_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/hubspot-forms/context";

const elementName = "hubspot-forms-modal";

@customElement(elementName)
export default class HubspotFormsModalElement
    extends UmbModalBaseElement<HubspotFormPickerModalData, HubspotFormPickerModalValue> {

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
    private _loading = false;

    @state()
    private _forms: Array<HubspotFormDtoModel> = [];

    @state()
    private _filteredForms: Array<HubspotFormDtoModel> = this._forms;

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
        this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        if (!this.#hubspotFormsContext || !this.#settingsModel) return;

        this._serviceStatus = {
            isValid: this.#settingsModel.isValid,
            type: this.#settingsModel.type!.value!,
            description: "",
            useOAuth: this.#settingsModel.isValid && this.#settingsModel.type!.value === "OAuth"
        }

        await this.#loadForms();
    }

    async #loadForms() {
        this._loading = true;
        const { data } = this._serviceStatus.useOAuth
            ? await this.#hubspotFormsContext.getFormsOAuth()
            : await this.#hubspotFormsContext.getFormsByApiKey();
        if (!data) return;

        this._forms = data.forms ?? [];
        this._filteredForms = data.forms ?? [];
        this._loading = false;

        if (!data.isValid || data.isExpired) {
            this._showError(data.error!);
        }
    }

    #handleFilterInput(event: UUIInputEvent) {
        let query = (event.target.value as string) || '';
        query = query.toLowerCase();

        const result = !query
            ? this._forms
            : this._forms.filter((form) => form.name?.includes(query));

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

    private _onSelect(form: HubspotFormDtoModel) {
        this.value = { form };
        this._submitModal();
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message },
        });
    }

    async #onConnect() {
        await this.#loadForms();
    }

    async #onRevoke() {
        this._filteredForms = [];
        await this.#checkApiConfiguration();
    }

    render() {
        return html`
            <umb-body-layout>
                <uui-box headline="HubSpot Forms">
                    ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${this._renderFilter()}
                    ${this._filteredForms.map((form) => {
                        return html`
                            <uui-ref-node-form
                              selectable
                              name=${form.name ?? ""}
                              detail=${form.fields ?? ""}
                              @selected=${() => this._onSelect(form)}>
                            </uui-ref-node-form>
                        `;
                    })}
                </uui-box>

                <uui-box headline="HubSpot API">
                    <hubspot-authorization @connect=${this.#onConnect} @revoke=${this.#onRevoke}> </hubspot-authorization>
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
            uui-box {
                margin-bottom: var(--uui-size-8);
            }

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