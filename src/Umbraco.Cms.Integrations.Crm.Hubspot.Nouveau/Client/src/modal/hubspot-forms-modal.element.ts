import { html, LitElement, property, customElement } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import type { HubspotFormPickerModalData, HubspotFormPickerModalValue } from "./hubspot.modal-token.js";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement('hubspot-forms-modal')
export default class HubspotFormsModalElement
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<HubspotFormPickerModalData, HubspotFormPickerModalValue> {

    @property({ attribute: false })
    modalContext?: UmbModalContext<HubspotFormPickerModalData, HubspotFormPickerModalValue>;

    @property({ attribute: false })
    data?: HubspotFormPickerModalData;

    private _handleCancel() {
        this.modalContext?.submit();
    }

    private _handleSubmit() {
        this.modalContext?.updateValue({ formId: "123" });
        this.modalContext?.submit();
    }

    render() {
        return html`
            <div>
                <h1>${this.modalContext?.data.headline ?? "Default headline"}</h1>
                <button @click=${this._handleCancel}>Cancel</button>
                <button @click=${this._handleSubmit}>Submit</button>
            </div>
        `;
    }
}