import { customElement, html, repeat, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { DynamicsFormPickerModalData, DynamicsFormPickerModalValue } from "./dynamics.modal-token";
import { DYNAMICS_CONTEXT_TOKEN } from "../context/dynamics.context";
import { FormDtoModel } from "@umbraco-integrations/dynamics/generated";

const elementName = "dynamics-forms-modal";

@customElement(elementName)
export default class DynamicsFormModalElement extends UmbModalBaseElement<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>{
    #dynamicsContext!: typeof DYNAMICS_CONTEXT_TOKEN.TYPE;
    
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
        await this.getForms();
    }

    async getForms(){
        const { data } = await this.#dynamicsContext.getForms("Both");
        if(!data) return;

        this._forms = data;
    }

    _onSelect(form: FormDtoModel){
        this.value = { selectedForm: form };
        this._submitModal();
    }

    render(){
        return html`
            <umb-body-layout>
                <uui-box headline="Dynamics Marketing Forms">
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
                        <p><b>Connected</b>: Thanhnd</p>
                    </div>
                    <div>
                        <uui-button 
                            look="primary" 
                            label="Connect"></uui-button>
                        <uui-button 
                            color="danger" 
                            look="secondary" 
                            label="Revoke"></uui-button>
                    </div>
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }
}

