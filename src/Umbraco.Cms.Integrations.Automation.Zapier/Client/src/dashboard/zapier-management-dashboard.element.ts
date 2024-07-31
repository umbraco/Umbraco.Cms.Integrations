import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html, nothing, customElement, state, query, property } from '@umbraco-cms/backoffice/external/lit';

const elementName = "zapier-management-dashboard";
@customElement(elementName)
export class ZapierManagementDashboardElement extends UmbLitElement {
    override render() {
		return html`
			<div>This is new Zapier dashboard</div>
		`;
	}
}

export default ZapierManagementDashboardElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: ZapierManagementDashboardElement;
	}
}