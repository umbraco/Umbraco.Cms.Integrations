import {
    LitElement,
    html,
    property,
    customElement
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import type { 
    HubSpotFormPickerModalData, 
    HubSpotFormPickerModalValue } 
from "../tokens/hubspot-forms-modal.token";

@customElement("hubspot-forms-modal")
export class HubSpotFormsModalElement 
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<HubSpotFormPickerModalData, HubSpotFormPickerModalValue> {

    @property({ attribute: false })
    modalContext?: UmbModalContext<HubSpotFormPickerModalData, HubSpotFormPickerModalValue>;

    @property({ attribute: false })
    data?: HubSpotFormPickerModalData;

    render() {
        return html`
            <div>
                <h1>TEST</h1>
            </div>
        `;
    }

}

export default HubSpotFormsModalElement;

declare global {
    interface HTMLElementTagNameMap {
        "hubspot-forms-modal": HubSpotFormsModalElement
    }
}