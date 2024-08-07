import { customElement, html, css, property, state, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const elementName = "dynamics-form-picker";

@customElement(elementName)
export class DynamicsFormPickerPropertyEditor extends UmbLitElement implements UmbPropertyEditorUiElement {
    
}

export default DynamicsFormPickerPropertyEditor;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsFormPickerPropertyEditor;
    }
}