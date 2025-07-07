import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { DynamicsService, OAuthRequestDtoModel, V1Service } from "@umbraco-integrations/dynamics/generated";

export class DynamicsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getForms(module: string){
        const { data, error } = await tryExecute(this, DynamicsService.getForms({
            query: {
                module
            }
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken() {
        const { data, error } = await tryExecute(this, DynamicsService.deleteFormsRevokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecute(this, DynamicsService.getFormsAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkOauthConfiguration() {
        const { data, error } = await tryExecute(this, DynamicsService.getFormsOauthConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oAuthRequestDtoModel: OAuthRequestDtoModel) {
        const { data, error } = await tryExecute(this, DynamicsService.postFormsAccessToken({ body: oAuthRequestDtoModel }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getEmbedCode(formId: string) {
        const { data, error } = await tryExecute(this, DynamicsService.getFormsEmbedCode({
            query: {
                formId
            }
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getSystemUserFullName() {
        const { data, error } = await tryExecute(this, DynamicsService.getFormsSystemUserFullname());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string) {
        const { data, error } = await tryExecute(this, V1Service.getUmbracoApiDynamicsAuthorization({
            query: {
                code
            }
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default DynamicsRepository;