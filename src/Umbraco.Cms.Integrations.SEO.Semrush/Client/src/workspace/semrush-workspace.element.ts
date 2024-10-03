import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html, nothing, customElement, state } from '@umbraco-cms/backoffice/external/lit';
import { SEMRUSH_CONTEXT_TOKEN } from '../context/semrush.context';
import type { UmbTableColumn, UmbTableItem } from '@umbraco-cms/backoffice/components';
import { UUIPaginationEvent } from '@umbraco-cms/backoffice/external/uui';
import { UmbPaginationManager } from "@umbraco-cms/backoffice/utils";

const elementName = "semrush-workspace-view";
@customElement(elementName)
export class SemrushWorkspaceElement extends UmbLitElement {
    constructor() {
        super();
      }

    render(){
        return html`
            <div>ABCD</div>
        `;
    }
}
export default SemrushWorkspaceElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: SemrushWorkspaceElement;
	}
}