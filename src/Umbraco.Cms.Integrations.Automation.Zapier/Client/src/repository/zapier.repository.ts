import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ZapierService } from "@umbraco-integrations/zapier/generated";

export class ZapierRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getAll() {
        const { data, error } = await tryExecute(this, ZapierService.getSubscriptionHooks());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentByType(alias: string) {
        const { data, error } = await tryExecute(this, ZapierService.getContentByType({
            path: {
                alias
            }
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentTypes() {
        const { data, error } = await tryExecute(this, ZapierService.getContentTypes());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkFormsExtensionInstalled() {
        const { data, error } = await tryExecute(this, ZapierService.getCheckFormExtension());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async updatePreferences() {
        const { data, error } = await tryExecute(this, ZapierService.postUpdateSubscription());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateUser() {
        const { data, error } = await tryExecute(this, ZapierService.postValidateUser());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default ZapierRepository;