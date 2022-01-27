### In DEBUG mode use following post build events for development:
set UmbracoCmsIntegrationsTestsiteV8Path=$(SolutionDir)\Umbraco.Cms.Integrations.Testsite.V8
set HubspotDir=%UmbracoCmsIntegrationsTestsiteV8Path%\App_Plugins\UmbracoCms.Integrations\Crm\Hubspot
if not exist %HubspotDir% mkdir -p %HubspotDir%
xcopy "$(ProjectDir)App_Plugins\UmbracoCms.Integrations\Crm\Hubspot" "%HubspotDir%" /e /y