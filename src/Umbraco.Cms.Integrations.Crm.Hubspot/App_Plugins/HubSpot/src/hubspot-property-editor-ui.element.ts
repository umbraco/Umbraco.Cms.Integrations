import { LitElement, html, css, customElement, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";
import {
    UMB_MODAL_MANAGER_CONTEXT
} from "@umbraco-cms/backoffice/modal";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { HUBSPOT_FORM_PICKER_MODAL } from "./modal/tokens/hubspot-forms-modal.token";
import type { FormModel } from "./models/FormModel";

@customElement('hubspot-property-editor-ui')
export class HubSpotPropertyEditorUIElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

    @property({ type: String })
    public value = "";

    @state()
    form?: FormModel;

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
    }

    render() {
        return html`
            ${
                this.form !== undefined
                    ? html`
                        <uui-ref-node-form
                            name="${this.form.name}"
                            alias="${this.form.name}">
                            <uui-action-bar slot="actions">
                                <uui-button label="Remove">Remove</uui-button>
                            </uui-action-bar>
                        </uui-ref-node-form>
                    `
                    : html`<uui-button look="placeholder" label="Add" @click=${() => this.#loadForms()}>Add</uui-button>`
            }
        `;
    }

    #loadForms() {
        this.#modalManagerContext?.open(this, HUBSPOT_FORM_PICKER_MODAL, {
            data: {
                headline: "My modal headline",
            },
        });
        
        //const modalContext = 
        
        // this.#modalManagerContext?.open(this, HUBSPOT_FORMS_PICKER_MODAL, {
        //     value: {
        //         key: "test",
        //     }
        // });
        
        // modalContext
        //     ?.onSubmit()
        //     .then((value) => {
        //         console.log(value);
        //     })
        //     .catch(() => undefined);
    }

    static styles = [
        css`
            uui-button { 
                width: 100% ;
            }
            uui-ref-node-form {
                width: 40%;
            }
        `
    ];    
}

export default HubSpotPropertyEditorUIElement;

declare global {
    interface HTMLElementTagNameMap {
        'hubspot-property-editor-ui': HubSpotPropertyEditorUIElement;
    }
}