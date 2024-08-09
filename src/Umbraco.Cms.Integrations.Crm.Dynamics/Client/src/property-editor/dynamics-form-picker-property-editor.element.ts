import { customElement, html, css, property, state, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { FormDtoModel } from "@umbraco-integrations/dynamics/generated";
import { DynamicsFormPickerConfiguration } from "../types/types";
import { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { DYNAMICS_MODAL_TOKEN } from "../modal/dynamics.modal-token";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";

const elementName = "dynamics-form-picker";

@customElement(elementName)
export class DynamicsFormPickerPropertyEditor extends UmbLitElement implements UmbPropertyEditorUiElement {
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;
    @state()
	private _config?: DynamicsFormPickerConfiguration;

    @property({ attribute: false })
	public set config(config: UmbPropertyEditorConfigCollection) {
		this._config = this.#mapDataTypeConfigToCollectionConfig(config);
	}

    #mapDataTypeConfigToCollectionConfig(
		config: UmbPropertyEditorConfigCollection | undefined,
	) : DynamicsFormPickerConfiguration {
		return {
			module: config?.getValueByAlias('modules'),
		};
	}

    @property({ type: String })
    public value = "";

    @state()
    private selectedForm: FormDtoModel | undefined;

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
        this.consumeContext(DYNAMICS_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#dynamicsContext = context;
            // this.observe(context.settingsModel, (settingsModel) => {
            //     this.#settingsModel = settingsModel;
            // });
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        if (this.value == null || this.value.length == 0) return;

        await this.#checkOAuthConfiguration();
    }

    async #checkOAuthConfiguration(){
        const {data} = await this.#dynamicsContext.checkOauthConfiguration();
        if(!data) return;
        if(!data.isAuthorized) this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.");

        await this.#getForm();
    }

    async #getForm(){
        const model: FormDtoModel = JSON.parse(JSON.stringify(this.value));
        this.selectedForm = model;
    }

    deleteForm(formId: string){
        
    }

    private async _openModal() {
        const pickerContext = this.#modalManagerContext?.open(this, DYNAMICS_MODAL_TOKEN, {
            data: {
                headline: "Dynamics Forms",
                config: this._config
            },
        });

        const data = await pickerContext?.onSubmit();
        if (!data) return;

        this.value = JSON.stringify(data.selectedForm);
        this.selectedForm = data.selectedForm;
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message }
        });
    }

    render() {
        return html`
            <div>
                <uui-button
                    class="add-button"
                    @click=${this._openModal}
                    label=${this.localize.term('general_add')}
                    look="placeholder"></uui-button>
            </div>
            ${this.selectedForm ? 
                html`
                    <div>
                        <uui-ref-node-form name=${this.selectedForm!.name}>
                            <uui-action-bar slot="actions">
                                <uui-button label="Remove" @click=${() => this.deleteForm(this.selectedForm!.id)}>Remove</uui-button>
                            </uui-action-bar>
                        </uui-ref-node-form>
                    </div>
                ` : 
                html``}
        `;
    }

    static styles = [
        css`
            .add-button {
                width: 100%;
            }
        `];
}

export default DynamicsFormPickerPropertyEditor;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsFormPickerPropertyEditor;
    }
}