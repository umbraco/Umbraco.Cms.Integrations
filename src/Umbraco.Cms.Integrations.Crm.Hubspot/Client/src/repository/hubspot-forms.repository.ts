import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { FormsService, type OAuthRequestDtoModel } from "@umbraco-integrations/hubspot-forms/generated";

export class HubspotFormsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecute(this, FormsService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkApiConfiguration() {
        const { data, error } = await tryExecute(this, FormsService.getCheckConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oauthRequestDto: OAuthRequestDtoModel) {
        const { data, error } = await tryExecute(this, FormsService.postGetAccessToken({
            body: oauthRequestDto
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateAccessToken() {
        const { data, error } = await tryExecute(this, FormsService.getValidateAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken() {
        const { data, error } = await tryExecute(this, FormsService.postRefreshAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken() {
        const { data, error } = await tryExecute(this, FormsService.postRevokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getFormsByApiKey() {
        const { data, error } = await tryExecute(this, FormsService.getFormsByApiKey());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getFormsOAuth() {
        const { data, error } = await tryExecute(this, FormsService.getFormsOAuth());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}