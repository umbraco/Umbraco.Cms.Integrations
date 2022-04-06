# Umbraco.Cms.Integrations.Automation.Zapier

This integration provides a dashboard interface that allows users to map content types with Zap triggers webhooks.

When the content type gets published, the details of the configuration are looked up into the database and if a record is found, 
will send a request to the Zap, triggering the matching actions.

A Zap is an automated workflow that connects various apps and services together. Each Zap consists of a trigger and one or more actions.

## Prerequisites

Requires minimum versions of Umbraco:

- CMS V8: 8.5.4
- CMS V9: 9.0.1

## How To Use

### Authentication

For this integration, the authentication is managed on Zapier's side. 

When creating a Zap trigger, you will be prompted to enter a username, password and the URL for your Umbraco website.

Then the Umbraco application existing in the Zapier marketplace will validate the credentials entered and return a message in case the validation fails.

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
* Registered Webhooks - list of created content configurations

Each content type can only have one webhook configuration attached. After a configuration has been added for a specific content type, the content type item gets removed from the input field, 
and it can be restored by deleting the configuration from the database.

The _Trigger Webhook_ action will send a test request to the Zap trigger, enabling the preview of requests in the Zap setup workflow.
