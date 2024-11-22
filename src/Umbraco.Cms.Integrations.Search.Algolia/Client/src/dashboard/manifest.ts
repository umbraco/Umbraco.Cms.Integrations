const dashboardManifest: UmbExtensionManifest = {
    type: "dashboard",
    alias: "Algolia.Dashboard",
    name: "Algolia Search Management",
    element: () => import('./search-management-dashboard/algolia-dashboard.element.js'),
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