import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type HubSpotFormPickerModalData = {
    headline: string;
};

export type HubSpotFormPickerModalValue = {
    name: string;
};

export const HUBSPOT_FORM_PICKER_MODAL = new UmbModalToken<HubSpotFormPickerModalData, HubSpotFormPickerModalValue>(
    "HubSpot.FormPicker.Modal", {
        modal: {
            type: "sidebar",
            size: "medium"
        }
    }
);