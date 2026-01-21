import type { ManifestLocalization } from "@umbraco-cms/backoffice/localization";

const enLocalization: ManifestLocalization = {
    type: "localization",
    alias: "Umb.Localization.GoogleSearchConsole.URLInspectionTool.en", 
    name: "GoogleSearchConsole URL Inspection Tool English Localization",
    meta: {
        culture: "en"
    },
    js: () => import("./en")
}

export const manifest: ManifestLocalization = enLocalization;
