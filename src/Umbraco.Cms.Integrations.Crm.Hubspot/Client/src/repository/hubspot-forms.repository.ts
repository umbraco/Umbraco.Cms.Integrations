import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { FormsService, type OAuthRequestDtoModel } from "@umbraco-integrations/hubspot-forms/generated";

export class HubspotFormsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkApiConfiguration() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.checkConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oauthRequestDto: OAuthRequestDtoModel) {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.getAccessToken({
            requestBody: oauthRequestDto
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateAccessToken() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.validateAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.refreshAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.revokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getFormsByApiKey() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.getAll());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getFormsOAuth() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.getAllOauth());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}