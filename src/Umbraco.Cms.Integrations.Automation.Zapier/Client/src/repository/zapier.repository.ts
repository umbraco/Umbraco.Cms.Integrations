import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Zapier, type SubscriptionDtoModel, type UserModel } from "@umbraco-integrations/zapier/generated";

export class ZapierRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getAll() {
        const { data, error } = await tryExecute(this, Zapier.getSubscriptionHooks());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentByType(alias: string) {
        const { data, error } = await tryExecute(this, Zapier.getContentByType({
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
        const { data, error } = await tryExecute(this, Zapier.getContentTypes());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async checkFormsExtensionInstalled() {
        const { data, error } = await tryExecute(this, Zapier.getCheckFormExtension());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async updatePreferences(subscription: SubscriptionDtoModel) {
        const { data, error } = await tryExecute(this, Zapier.postUpdateSubscription({ body: subscription }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateUser(user: UserModel) {
        const { data, error } = await tryExecute(this, Zapier.postValidateUser({ body: user }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default ZapierRepository;