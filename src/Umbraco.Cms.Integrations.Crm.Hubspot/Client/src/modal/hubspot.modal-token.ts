import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { HubspotFormDtoModel } from "@umbraco-integrations/hubspot-forms/generated";

export type HubspotFormPickerModalData = {
    headline: string;
}

export type HubspotFormPickerModalValue = {
    form: HubspotFormDtoModel;
}

export const HUBSPOT_FORMS_MODAL_TOKEN = new UmbModalToken<HubspotFormPickerModalData, HubspotFormPickerModalValue>("HubspotForms.Modal", {
    modal: {
        type: "sidebar",
        size: "small"
    }
});