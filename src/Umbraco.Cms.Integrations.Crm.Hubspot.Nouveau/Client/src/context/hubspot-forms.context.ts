import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

import { HubspotFormsRepository } from "../repository/hubspot-forms.repository.js";

export class HubspotFormsContext extends UmbControllerBase {
    #repository: HubspotFormsRepository;

    constructor(host: UmbControllerHost) {
        super(host);

        this.provideContext(HUBSPOT_FORMS_CONTEXT_TOKEN, this);
        this.#repository = new HubspotFormsRepository(host);
    }

    async checkApiConfiguration() {
        return await this.#repository.checkApiConfiguration();
    }

    async validateAccessToken() {
        return await this.#repository.validateAccessToken();
    }
}

export default HubspotFormsContext;

export const HUBSPOT_FORMS_CONTEXT_TOKEN =
    new UmbContextToken<HubspotFormsContext>(HubspotFormsContext.name);