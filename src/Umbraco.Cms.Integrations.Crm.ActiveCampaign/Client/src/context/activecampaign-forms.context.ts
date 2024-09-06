import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbObjectState } from "@umbraco-cms/backoffice/observable-api";
import { ActiveCampaignFormsRepository } from "../repository/activecampaign-forms.repository";
import { ApiAccessDtoModel } from "../../generated/types.gen";

export class ActiveCampaignFormsContext extends UmbControllerBase {
    #repository: ActiveCampaignFormsRepository;
    #configurationModel = new UmbObjectState<ApiAccessDtoModel | undefined>(undefined);
    configurationModel = this.#configurationModel.asObservable();

    constructor(host: UmbControllerHost) {
        super(host);

        this.provideContext(ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN, this);
        this.#repository = new ActiveCampaignFormsRepository(host);
    }

    async hostConnected() {
        super.hostConnected();
        this.checkApiAccess();
    }

    async checkApiAccess() {
        const { data } = await this.#repository.checkApiAccess();

        this.#configurationModel.setValue(data);
    }

    async getForms(page?: number) {
        return await this.#repository.getForms(page);
    }

    async getForm(id: string) {
        return await this.#repository.getForm(id);
    }
}

export default ActiveCampaignFormsContext;

export const ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN =
    new UmbContextToken<ActiveCampaignFormsContext>(ActiveCampaignFormsContext.name);