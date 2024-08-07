import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { DynamicsService } from "@umbraco-integrations/dynamics/generated";

export class DynamicsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getForms(module: string){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.getForms({
            module: module
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken(){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.revokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl(){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkOauthConfiguration(){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.checkOauthConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.getAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getEmbedCode(formId: string){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.getEmbedCode({
            formId: formId
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getSystemUserFullName(){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.getSystemUserFullName());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string){
        const { data, error } = await tryExecuteAndNotify(this, DynamicsService.oauth({
            code: code
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default DynamicsRepository;