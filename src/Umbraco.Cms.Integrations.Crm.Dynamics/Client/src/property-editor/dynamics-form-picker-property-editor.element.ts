import { customElement, html, css, property, state, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { DynamicsModuleModel, FormDtoModel, OAuthConfigurationDtoModel } from "@umbraco-integrations/dynamics/generated";
import { DynamicsFormPickerConfiguration } from "../types/types";
import { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { DYNAMICS_MODAL_TOKEN } from "../modal/dynamics.modal-token";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";

const elementName = "dynamics-form-picker";

@customElement(elementName)
export class DynamicsFormPickerPropertyEditor extends UmbLitElement implements UmbPropertyEditorUiElement {
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    #settingsModel?: OAuthConfigurationDtoModel;
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
            this.observe(context.settingsModel, (settingsModel) => {
                this.#settingsModel = settingsModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        setTimeout(() => {
            this.#checkConfiguration();
        }, 3000);
    }

    #checkConfiguration() {
        if (this.value == null || this.value.length == 0) return;

        if(!this.#settingsModel) return;
        if(!this.#settingsModel.isAuthorized) this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type.");

        this.selectedForm = JSON.parse(JSON.stringify(this.value));
    }

    #deleteForm(){
        this.value = "";
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    private async _openModal() {
        const module = this._config?.module == "Both" ? "Outbound | Real-Time" : this._config?.module;

        const pickerContext = this.#modalManagerContext?.open(this, DYNAMICS_MODAL_TOKEN, {
            data: {
                headline: `Dynamics Forms - ${module} Marketing Forms`,
                module: this._config?.module!
            }
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
        ${this.value == null || this.value.length == 0 ? 
            html`
                <div>
                    <uui-button
                        class="add-button"
                        @click=${this._openModal}
                        label=${this.localize.term('general_add')}
                        look="placeholder"></uui-button>
                </div>
            ` : 
            html`
            ${this.selectedForm ? 
                html`
                    <div>
                        <uui-ref-node-form name=${this.selectedForm.name}>
                            <uui-action-bar slot="actions">
                                <uui-button label="Remove" @click=${this.#deleteForm}>Remove</uui-button>
                            </uui-action-bar>
                        </uui-ref-node-form>
                    </div>
                ` : 
                html`
                    <div class="center loader"><uui-loader></uui-loader></div>
                `}
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

export default DynamicsFormPickerPropertyEditor;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsFormPickerPropertyEditor;
    }
}