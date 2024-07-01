import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { FormsService } from "@umbraco-integrations/hubspot-forms/generated";

export class HubspotFormsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async checkApiConfiguration() {
        const { data, error } = await tryExecuteAndNotify(this, FormsService.checkConfiguration());

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
}