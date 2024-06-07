import { ManifestDashboard } from "@umbraco-cms/backoffice/extension-registry";

const dashboardManifest: ManifestDashboard = {
    type: "dashboard",
    name: "Algolia Search Management",
    alias: "Algolia.Dashboard",
    elementName: "algolia-dashboard-element",
    js: () => import("./search-management-dashboard/algolia-dashboard.element"),
    meta: {
        label: "Algolia Search Management",
        pathname: "algolia-search-management"
    },
    conditions: [
        {
            alias: "Umb.Condition.SectionAlias",
            match: "Umb.Section.Settings"
        }
    ]
};

export const manifest = dashboardManifest;