import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html, property, state, when } from "@umbraco-cms/backoffice/external/lit";

const elementName = "dynamics-authorization";

@customElement(elementName)
export class DynamicsAuthorizationElement extends UmbElementMixin(LitElement){
    constructor() {
        super();

        // this.consumeContext(SHOPIFY_CONTEXT_TOKEN, (context) => {
        //     if (!context) return;
        //     this.#shopifyContext = context;
        //     this.observe(context.settingsModel, (settingsModel) => {
        //         this.#settingsModel = settingsModel;
        //     });
        // });
    }

    async connectedCallback() {
        super.connectedCallback();
    }

    render(){
        return html`
            <div>
                <p>Connected: Thanhnd</p>
            </div>
            <div>
                <uui-button 
                    look="primary" 
                    label="Connect"></uui-button>
                <uui-button 
                    color="danger" 
                    look="secondary" 
                    label="Revoke"></uui-button>
            </div>
        `;
    }
}

export default DynamicsAuthorizationElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: DynamicsAuthorizationElement;
	}
}