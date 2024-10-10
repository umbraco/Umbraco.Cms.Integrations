import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { AuthorizationResponseDtoModel } from "@umbraco-integrations/semrush/generated";

export type SemrushModalData = {
    headline: string;
    authResponse: AuthorizationResponseDtoModel | undefined;
}

export type SemrushModalValue = {
    authResponse: AuthorizationResponseDtoModel | undefined;
}

export const SEMRUSH_MODAL_TOKEN = new UmbModalToken<SemrushModalData, SemrushModalValue>("Semrush.Modal", {
    modal: {
        type: "sidebar",
        size: "small"
    }
});