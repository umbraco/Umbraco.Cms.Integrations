# CommerceTools
This integration provides you with a custom product picker, allowing you to pick a CommerceTool provided product or category, convert the product or category into a content item in your Umbraco backoffice and publish it to your Umbraco webpage!

CommerceTool APIs are used to build a custom property editor, allowing an editor to pick a CommerceTool provided product or category and associate the identifier of the selected item with a Umbraco content item. 

A property value converter converts the identifier into a strongly typed model, which is hydrated via a further API call and can be used for rendering to a page or including in a further API response. 

**Want to know more about how the integration is made?**

If you want to see the details on how the integration to CommerceTool is made or just follow the example of extending Umbraco with a third-party system you can take a look at the [blog post](https://umbraco.com/blog/integrating-umbraco-cms-with-commercetools/) supplementing this integration. 