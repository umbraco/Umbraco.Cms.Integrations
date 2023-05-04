# URL Inspection Tool for Google Search Console 

With this integration to URL Inspection Tool for Google Search Console, you get a tool that ensures your Umbraco website(s) and URLs are performant!

With this integration, you can add a page or template-level insights and ongoing checks for existing pages.

The request parameters include the URL you’d like to inspect and the URL of the property as defined in the Search Console.

The response includes analysis results containing information from Search Console, including index status, AMP, rich results, and mobile usability.

The usage quote is enforced per Search Console website property: 2000 queries per day / 600 queries per minute.

**Umbraco Content App**

The current integration is available as a content app for each content node named _URL Inspection_ area.  

By default, the inspection input fields are disabled and the values reflect the root URL of your website and of the current node, as any inspection requests take into consideration the current node. You can manually change the input values by selecting the _Edit_ action, and the request data can be adjusted to customize the results.

The inspection result consists of these fields, but these can vary depending on the tracking level of your property:
- Inspection Result Link - link to Search Console URL inspection. 
- Index Status- the result of the index status analysis. For more information, please check the [Index coverage report documentation](https://support.google.com/webmasters/answer/7440203).
- AMP - the result of the AMP analysis. Absent if the page is not an AMP page. Key elements returned include:
- Mobile Usability- the result of the mobile usability analysis.
- Rich Results - the result of the rich results analysis. Absent if there are no rich results found.

Common properties returned include:
- Link to inspection view per section
- List of issues found
- Verdict: Unspecified, Pass, Partial, Fail, Neutral

You can define cultures and hostnames, this will reflect in the type of field for the Inspect URL property.

**Want to know more about this integration?**

If you want to see the details on how the integration to Google Search Console - URL Inspection Tool is made or just follow the example of extending Umbraco with a third-party system you can take a look at the [blog post](https://umbraco.com/blog/integrating-umbraco-cms-with-google-search-console-url-inspection-tool/) supplementing this integration.

![content-app](https://github.com/umbraco/Umbraco.Cms.Integrations/blob/docs/integrations-readmes/src/Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.UrlInspectionTool/docs/images/content-app.png)

![connect](https://github.com/umbraco/Umbraco.Cms.Integrations/blob/docs/integrations-readmes/src/Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.UrlInspectionTool/docs/images/connect.png)

![inspect](https://github.com/umbraco/Umbraco.Cms.Integrations/blob/docs/integrations-readmes/src/Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.UrlInspectionTool/docs/images/inspect.png)

![cultures](https://github.com/umbraco/Umbraco.Cms.Integrations/blob/docs/integrations-readmes/src/Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.UrlInspectionTool/docs/images/cultures.png)

![links](https://github.com/umbraco/Umbraco.Cms.Integrations/blob/docs/integrations-readmes/src/Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.UrlInspectionTool/docs/images/links.png)