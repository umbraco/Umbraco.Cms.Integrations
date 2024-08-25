import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, css, customElement, html, nothing, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN } from "@umbraco-integrations/activecampaign-forms/context";
import { ApiAccessDtoModel, FormDtoModel } from "@umbraco-integrations/activecampaign-forms/generated";
import { ACTIVECAMPAIGN_FORMS_MODAL_TOKEN } from "../modal/activecampaign.modal-token";

const elementName = "activecampaign-form-picker";

@customElement(elementName)
export default class ActiveCampaignFormPickerElement extends UmbElementMixin(LitElement) {
    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;
    #activecampaignFormsContext!: typeof ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN.TYPE;
    #configurationModel?: ApiAccessDtoModel;

    @property({ type: String })
    public value = "";

    
    @state()
    private _form: FormDtoModel = {
        id: "",
        name: ""
    };

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
        this.consumeContext(ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN, (context) => {
            if (!context) return;

            this.#activecampaignFormsContext = context;
            this.observe(context.configurationModel, (configurationModel) => {
                this.#configurationModel = configurationModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        if (this.value == null || this.value.length == 0) return;

        const { data } = await this.#activecampaignFormsContext.getForm(this.value);
        if (!data) return;

        console.log(data);

        this._form = {
            id: data.form.id,
            name: data.form.name
        };
    }

    private async _openModal() {
        const pickerContext = this.#modalManagerContext?.open(this, ACTIVECAMPAIGN_FORMS_MODAL_TOKEN, {
            data: {
                headline: "ActiveCampaign Forms",
            },
        });

        const data = await pickerContext?.onSubmit();
        if (!data) return;

        this._form = {
            id: data.form.id,
            name: data.form.name
        };

        this.value = data.form.id;
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private _renderWarning() {
        return html`<div class="warning">Invalid API configuration.</div>`;
    }

    #deleteForm() {
        this.value = "";
        this.dispatchEvent(new CustomEvent('property-value-change'));
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
                    <uui-ref-node-form selectable name=${this._form?.name ?? ""}>
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" @click=${this.#deleteForm}>Remove</uui-button>
                        </uui-action-bar>
                    </uui-ref-node-form>  
                `}
            ${!this.#configurationModel?.isApiConfigurationValid ? this._renderWarning() : nothing}
		`;
    }

    static styles = [
        css`
            .add-button {
                width: 100%;
            }

            .warning {
                background-color: #fff3cd;
                border-color: #ffeeba;
                position: relative;
                padding: .75rem 1.25rem;
                margin-top: 10px;
                border: 1px solid transparent;
                border-radius: .25rem;
            }
        `];
}

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: ActiveCampaignFormPickerElement;
    }
}