import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { FormDtoModel } from "@umbraco-integrations/activecampaign-forms/generated";

export type ActiveCampaignFormPickerModalData = {
    headline: string;
}

export type ActiveCampaignFormPickerModalValue = {
    form: FormDtoModel;
}

export const ACTIVECAMPAIGN_FORMS_MODAL_TOKEN = new UmbModalToken<ActiveCampaignFormPickerModalData, ActiveCampaignFormPickerModalValue>("ActiveCampaignForms.Modal", {
    modal: {
        type: "sidebar",
        size: "medium"
    }
});