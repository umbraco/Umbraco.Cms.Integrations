import { html, css, LitElement, property, state, customElement } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import { HUBSPOT_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/hubspot-forms/context";
import type { HubspotFormPickerModalData, HubspotFormPickerModalValue } from "./hubspot.modal-token.js";
import { HubspotServiceStatus } from "../models/hubspot-service.model.js";
import { HubspotFormDtoModel } from "@umbraco-integrations/hubspot-forms/generated";
import { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";

const elementName = "hubspot-forms-modal";

@customElement(elementName)
export default class HubspotFormsModalElement
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<HubspotFormPickerModalData, HubspotFormPickerModalValue> {

    #hubspotFormsContext!: typeof HUBSPOT_FORMS_CONTEXT_TOKEN.TYPE;

    @property({ attribute: false })
    modalContext?: UmbModalContext<HubspotFormPickerModalData, HubspotFormPickerModalValue>;

    @property({ attribute: false })
    data?: HubspotFormPickerModalData;

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
            this.#hubspotFormsContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        this.#checkApiConfiguration();
    }

    async #checkApiConfiguration() {
        const { data } = await this.#hubspotFormsContext?.checkApiConfiguration();
        if (!data) return;

        this._serviceStatus = {
            isValid: data!.isValid,
            type: data!.type?.value!,
            description: "",
            useOAuth: data!.isValid && data!.type?.value! == "OAuth"
        }

        await this.#loadForms();
    }

    async #loadForms() {
        this._loading = true;
        const { data } = this._serviceStatus.useOAuth
            ? await this.#hubspotFormsContext.getFormsOAuth()
            : await this.#hubspotFormsContext.getFormsByApiKey();
        if (!data) return;

        this._forms = data.forms!;
        this._filteredForms = data.forms!;
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

    private _onClose() {
        this.modalContext?.submit();
    }

    private _onSelect(form: HubspotFormDtoModel) {
        this.modalContext?.updateValue({ form: form });
        this.modalContext?.submit();
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message },
        });
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
                              name=${form.name}
                              detail=${form.fields}
                              @selected=${() => this._onSelect(form)}>
                            </uui-ref-node-form>
                        `;
                    })}
                </uui-box>

                <uui-box headline="HubSpot API">
                    <hubspot-authorization></hubspot-authorization>
                </uui-box>

                <div slot="actions">
                    <uui-button id="close" label="Close" @click="${this._onClose}">Close</uui-button>
                </div>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
            uui-box {
                margin-bottom: 10px;
            }

            #filter {
                width: 100%;
                margin-bottom: 10px;
            }

            uui-icon {
                margin: auto;
                margin-left: 5px;
            }
        `];
}