import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import {
    LitElement,
    css,
    customElement,
    html,
    property
} from "@umbraco-cms/backoffice/external/lit";

const elementName = "dynamics-form-picker";
@customElement(elementName)
export default class DynamicsFormPickerElement extends UmbElementMixin(LitElement) {

    @property({ type: String })
    public value = "";

    private async _openModal() {
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
                   <p>Form goes here</p> 
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

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsFormPickerElement;
    }
}