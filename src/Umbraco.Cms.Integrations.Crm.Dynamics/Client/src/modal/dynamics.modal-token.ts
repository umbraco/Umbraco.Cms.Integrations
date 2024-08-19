import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { FormDtoModel } from "@umbraco-integrations/dynamics/generated";
import { DynamicsFormPickerConfiguration } from "../types/types";

export type DynamicsFormPickerModalData = {
    headline: string;
    module: string;
}

export type DynamicsFormPickerModalValue = {
    selectedForm: FormDtoModel;
    iframeEmbedded: boolean;
}

export const DYNAMICS_MODAL_TOKEN = new UmbModalToken<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>("Dynamics.Modal", {
    modal: {
        type: "sidebar",
        size: "large"
    }
});