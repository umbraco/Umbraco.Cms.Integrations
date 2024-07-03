import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, customElement, html, css, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";
import { HUBSPOT_FORMS_MODAL_TOKEN } from "../modal/hubspot.modal-token.js";
import { ConfigDescription, type HubspotServiceStatus } from "../models/hubspot-service.model.js";
import { HUBSPOT_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/hubspot-forms/context";
import type { HubspotFormDtoModel } from "@umbraco-integrations/hubspot-forms/generated";

const elementName = "hubspot-form-picker";

@customElement(elementName)
export class HubspotFormPickerElement extends UmbElementMixin(LitElement) {

    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;
    #hubspotFormsContext!: typeof HUBSPOT_FORMS_CONTEXT_TOKEN.TYPE;

    @property({ type: String })
    public value = "";

    @state()
    private _form: HubspotFormDtoModel = {};

    @state()
    private _serviceStatus: HubspotServiceStatus = {
        isValid: false,
        type: "",
        description: "",
        useOAuth: false
    };

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
        this.consumeContext(HUBSPOT_FORMS_CONTEXT_TOKEN, (context) => {
            this.#hubspotFormsContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        if (this.value == null || this.value.length == 0) return;

        const { data } = await this.#hubspotFormsContext.checkApiConfiguration();
        if (!data || !data.type?.value) return;

        this._serviceStatus = {
            isValid: data.isValid,
            type: data.type.value,
            description: "",
            useOAuth: data.isValid && data.type.value === "OAuth"
        }

        if (!this._serviceStatus.isValid) {
            this._showError(ConfigDescription.none);
        }

        const dto: HubspotFormDtoModel = JSON.parse(JSON.stringify(this.value));
        this._form = {
            id: dto.id,
            name: dto.name,
            fields: dto.fields
        };
    }

    #deleteForm() {
        this.value = "";
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private async _openModal() {
        const pickerContext = this.#modalManagerContext?.open(this, HUBSPOT_FORMS_MODAL_TOKEN, {
            data: {
                headline: "HubSpot Forms",
            },
        });

        const data = await pickerContext?.onSubmit();
        if (!data) return;

        this.value = JSON.stringify(data.form);
        console.log(this.value);
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message },
        });
    }

    render() {
        return html`
            ${this.value == null || this.value.length == 0
                ? html`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term('general_add')}
				        look="placeholder"></uui-button>
                `
                : html`
                    <uui-ref-node-form selectable name=${this._form?.name ?? ""} detail=${this._form?.fields ?? ""}>
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" @click=${this.#deleteForm}>Remove</uui-button>
                        </uui-action-bar>
                    </uui-ref-node-form>  
                `}
		`;
    }

    static styles = [
        css`
            .add-button {
                width: 100%;
            }
        `];
}

export default HubspotFormPickerElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: HubspotFormPickerElement;
    }
}