# Umbraco.Cms.Integrations.Automation.Zapier

This integration provides a dashboard interface that allows users to vizualize registered subscription hooks. When a Zap is turned on, the subscription hook is saved into the database; turning off the Zap will remove the registered subscription hook.

When content gets published, the content type is looked up in the subscription hooks list from the database, and if a record is found, A POST request will be sent to the webhook URL with details of the current node. This eventually will cause the Zap's trigger to be invoked, triggering the assigned actions of the Zap.

A Zap is an automated workflow that connects various apps and services together. Each Zap consists of a trigger and one or more actions.

## Prerequisites

Requires minimum versions of Umbraco:

- CMS V8: 8.5.4
- CMS V9: 9.0.1

## How To Use

### Authentication

For this integration, the authentication is managed on Zapier's side by using the Umbraco marketplace app. 

The Umbraco app manages two types of events:
* New Form Submission - triggers when a form is submitted
* New Content Published - triggers when a new content has been published.

The trigger event to be used by this integration is _New Content Published_.

When creating the Zap trigger, you will be prompted to enter a username, password and the URL for your Umbraco website, or you can use instead an API key.
If the following setting is present, then the API key based authentication will take precendence and will be the main method of authorization.
```
<appSettings>
...
  <add key="Umbraco.Cms.Integrations.Automation.Zapier.ApiKey" value="[your_api_key]" />
...
</appSettings>
```

If no API key is present, then the Umbraco application will validate the credentials entered and return a message in case the validation fails.

If you want to extend the security layer, you can also specify a user group that the user trying to connect needs to be a part of, by adding the following 
setting in `Web.config`:

```
<appSettings>
...
  <add key="Umbraco.Cms.Integrations.Automation.Zapier.UserGroup" value="[your User Group]" />
...
</appSettings>
```

### Working With the Zapier Cms Integration
In the _Content_ area of the backoffice, find the _Zapier Integrations_ dashboard.

The dashboard is composed of two sections:
* Content Properties - Zapier details and input fields for adding content configurations
* Registered Subscription Hooks - list of registered entities.

Subscription hooks are split in two categories: 
* 1 = Content
* 2 = Form

The _Trigger Webhook_ action will send a test request to the Zap trigger, enabling the preview of requests in the Zap setup workflow.
