import type { ManifestDashboard } from '@umbraco-cms/backoffice/dashboard';

const dashboards: Array<ManifestDashboard> = [
	{
		type: 'dashboard',
		alias: 'Zapier.Management.Dashboard',
		name: 'Zapier Management Dashboard',
		element: () => import('./zapier-management-dashboard.element'),
		weight: 5,
		meta: {
			label: 'Zapier Integrations',
			pathname: 'zapier-management',
		},
		conditions: [
			{
				alias: 'Umb.Condition.SectionAlias',
				match: 'Umb.Section.Content',
			},
		],
	},
];

export const manifests: Array<UmbExtensionManifest> = [...dashboards];
