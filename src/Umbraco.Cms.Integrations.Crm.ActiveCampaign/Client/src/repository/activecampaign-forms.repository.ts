import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ActiveCampaignFormsService } from "@umbraco-integrations/activecampaign-forms/generated";

export class ActiveCampaignFormsRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async checkApiAccess() {
        const { data, error } = await tryExecute(this, ActiveCampaignFormsService.getApiAccess());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getForm(id: string) {
        const { data, error } = await tryExecute(this, ActiveCampaignFormsService.getFormsById({ path: { id } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getForms(page?: number, searchQuery?: string) {
        const { data, error } = await tryExecute(this, ActiveCampaignFormsService.getForms({ query: { page, searchQuery } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}