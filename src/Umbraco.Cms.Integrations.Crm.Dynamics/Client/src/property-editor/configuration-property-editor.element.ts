import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    LitElement,
    customElement,
    html,
    property,
    state,
    when
} from "@umbraco-cms/backoffice/external/lit";

const elementName = "dynamics-configuration";

@customElement(elementName)
export default class DynamicsConfigurationElement extends UmbElementMixin(LitElement) {

    @property({ type: String })
    public value = "";

    @state()
    showAuthTokenComponent: boolean = false;

    render() {
        return html`
            <p>Dynamics Configuration</p>
            ${when(this.showAuthTokenComponent, () => html`
                  <dynamics-authorization-code></dynamics-authorization-code>
                    `)}
        `;
    }
}

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsConfigurationElement;
    }
}