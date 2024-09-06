import type { ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: Array<ManifestTypes> = [{
	type: 'icons',
	name: 'ActiveCampaign Icon',
	alias: 'activecampaign.icon',
	js: () => import('./icons-dictionary.js'),
}];