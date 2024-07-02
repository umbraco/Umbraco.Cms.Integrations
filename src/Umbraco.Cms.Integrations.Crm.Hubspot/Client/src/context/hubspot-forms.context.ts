import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

import { HubspotFormsRepository } from "../repository/hubspot-forms.repository.js";
import { type OAuthRequestDtoModel } from "../../generated/types.gen.js";

export class HubspotFormsContext extends UmbControllerBase {
    #repository: HubspotFormsRepository;

    constructor(host: UmbControllerHost) {
        super(host);

        this.provideContext(HUBSPOT_FORMS_CONTEXT_TOKEN, this);
        this.#repository = new HubspotFormsRepository(host);
    }

    async getAuthorizationUrl() {
        return await this.#repository.getAuthorizationUrl();
    }

    async checkApiConfiguration() {
        return await this.#repository.checkApiConfiguration();
    }

    async getAccessToken(oauthRequestDto: OAuthRequestDtoModel) {
        return await this.#repository.getAccessToken(oauthRequestDto);
    }

    async validateAccessToken() {
        return await this.#repository.validateAccessToken();
    }

    async refreshAccessToken() {
        return await this.#repository.refreshAccessToken();
    }

    async revokeAccessToken() {
        return await this.#repository.revokeAccessToken();
    }

    async getFormsByApiKey() {
        return await this.#repository.getFormsByApiKey();
    }

    async getFormsOAuth() {
        return await this.#repository.getFormsOAuth();
    }
}

export default HubspotFormsContext;

export const HUBSPOT_FORMS_CONTEXT_TOKEN =
    new UmbContextToken<HubspotFormsContext>(HubspotFormsContext.name);