import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { LitElement, customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";

import { HUBSPOT_FORMS_MODAL_TOKEN } from "../modal/hubspot.modal-token";

const elementName = "hubspot-form-picker";

@customElement(elementName)
export class HubspotFormPickerElement extends UmbElementMixin(LitElement) {

    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

    @property({ type: String })
    public value = "";

    constructor() {
        super();
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
    }

    render() {
        return html`
            <p>Hello world!</p>
             <uui-button look="primary" label="Open modal" @click=${this._openModal}></uui-button>
        `;
    }

    private _openModal() {
        this.#modalManagerContext?.open(this, HUBSPOT_FORMS_MODAL_TOKEN, {
            data: {
                headline: "My modal headline",
            },
        });
    }
}

export default HubspotFormPickerElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: HubspotFormPickerElement;
    }
}