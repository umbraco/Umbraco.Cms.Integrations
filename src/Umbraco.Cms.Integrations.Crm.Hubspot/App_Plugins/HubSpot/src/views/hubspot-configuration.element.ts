import { LitElement, html, customElement } from "@umbraco-cms/backoffice/external/lit";
import { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement('hubspot-configuration-ui')
export class HubSpotConfigurationUIElement
    extends LitElement
    implements UmbPropertyEditorUiElement
{
    render() {
        return html`configuration monster`;
    }
}

export default HubSpotConfigurationUIElement;

declare global {
    interface HTMLElementTagNameMap {
        'hubspot-configuration-ui': HubSpotConfigurationUIElement;
    }
}