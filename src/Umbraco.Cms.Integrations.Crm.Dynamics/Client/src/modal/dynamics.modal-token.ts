import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import type { FormDtoModel } from "@umbraco-integrations/dynamics/generated";
import { DynamicsFormPickerConfiguration } from "../types/types";

export type DynamicsFormPickerModalData = {
    headline: string;
}

export type DynamicsFormPickerModalValue = {
    selectedForm : FormDtoModel;
}

export const DYNAMICS_MODAL_TOKEN = new UmbModalToken<DynamicsFormPickerModalData, DynamicsFormPickerModalValue>("Dynamics.Modal", {
    modal: {
        type: "sidebar",
        size: "large"
    }
});