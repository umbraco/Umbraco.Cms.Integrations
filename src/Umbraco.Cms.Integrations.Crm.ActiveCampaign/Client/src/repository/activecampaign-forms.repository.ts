import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { ActiveCampaignFormsService } from "@umbraco-integrations/activecampaign-forms/generated";

export class ActiveCampaignFormsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async checkApiAccess() {
        const { data, error } = await tryExecuteAndNotify(this, ActiveCampaignFormsService.checkApiAccess());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getForm(id: string) {
        const { data, error } = await tryExecuteAndNotify(this, ActiveCampaignFormsService.getForm({ id: id }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getForms(page?: number) {
        const { data, error } = await tryExecuteAndNotify(this, ActiveCampaignFormsService.getForms({ page: page }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}