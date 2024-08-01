import type { ManifestDashboard, ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';

const dashboards: Array<ManifestDashboard> = [
	{
		type: 'dashboard',
		alias: 'Zapier.Management.Dashboard',
		name: 'Zapier Management Dashboard',
		element: () => import('./zapier-management-dashboard.element'),
		weight: 20,
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

export const manifests: Array<ManifestTypes> = [...dashboards];
