import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Dynamics, OAuthRequestDtoModel, V1 } from "@umbraco-integrations/dynamics/generated";

export class DynamicsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getForms(module: string){
        const { data, error } = await tryExecute(this, Dynamics.getForms({
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
        const { data, error } = await tryExecute(this, Dynamics.deleteFormsRevokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecute(this, Dynamics.getFormsAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkOauthConfiguration() {
        const { data, error } = await tryExecute(this, Dynamics.getFormsOauthConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oAuthRequestDtoModel: OAuthRequestDtoModel) {
        const { data, error } = await tryExecute(this, Dynamics.postFormsAccessToken({ body: oAuthRequestDtoModel }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getEmbedCode(formId: string) {
        const { data, error } = await tryExecute(this, Dynamics.getFormsEmbedCode({
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
        const { data, error } = await tryExecute(this, Dynamics.getFormsSystemUserFullname());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string) {
        const { data, error } = await tryExecute(this, V1.getUmbracoApiDynamicsAuthorization({
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