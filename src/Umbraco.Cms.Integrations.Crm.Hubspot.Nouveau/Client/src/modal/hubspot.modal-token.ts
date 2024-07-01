import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type HubspotFormPickerModalData = {
    headline: string;
}

export type HubspotFormPickerModalValue = {
    formId: string;
}

export const HUBSPOT_FORMS_MODAL_TOKEN = new UmbModalToken<HubspotFormPickerModalData, HubspotFormPickerModalValue>("HubspotForms.Modal", {
    modal: {
        type: "sidebar",
        size: "small"
    }
});