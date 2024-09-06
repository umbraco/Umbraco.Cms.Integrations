import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN } from "../context/activecampaign-forms.context";
import { ApiAccessDtoModel } from "../../generated";

const elementName = "activecampaign-forms-configuration";

@customElement(elementName)
export default class ActiveCampaignConfigurationElement extends UmbElementMixin(LitElement) {
    #configurationModel?: ApiAccessDtoModel;

    constructor() {
        super();
        this.consumeContext(ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN, (context) => {
            if (!context) return;

            this.observe(context.configurationModel, (configurationModel) => {
                this.#configurationModel = configurationModel;
            });
        });
    }

    render() {
        return html`
            <div>
                <p>
                    ${this.#configurationModel?.isApiConfigurationValid
                        ? html`Connected. Account name: <b>${this.#configurationModel.account}</b>`
                        : `Invalid API configuration.`
                    }
                </p>
            </div>
        `;
    }
}