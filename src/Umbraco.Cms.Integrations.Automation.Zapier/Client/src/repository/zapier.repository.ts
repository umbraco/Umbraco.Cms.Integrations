import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { ZapierService } from "@umbraco-integrations/zapier/generated";

export class ZapierRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getAll(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.getAll());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentByType(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.getContentByType());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentTypes(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.getContentTypes());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async isFormsExtensionInstalled(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.isFormsExtensionInstalled());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async updatePreferences(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.updatePreferences());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateUser(){
        const { data, error } = await tryExecuteAndNotify(this, ZapierService.validateUser());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default ZapierRepository;