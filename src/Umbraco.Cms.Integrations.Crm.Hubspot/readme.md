# Umbraco.Cms.Integrations.Crm.Hubspot

This integration provides a form picker and rendering component for forms managed within a [Hubspot](https://www.hubspot.com/) account.

## Prerequisites

Requires minimum version of Umbraco CMS: 8.5.4.

## How To Use

### Authentication

The package supports two modes of authentication:

- API Key
- OAuth

#### API Key

Log into your HubSpot account, go to _Settings > Integrations > API Key_ and create an API key.

Add this to an app setting in `Web.config`:

```
  <appSettings>
    ...
    <add key="Umbraco.Cms.Integrations.Crm.Hubspot.DefaultLanguage" value="[your API key]" />
    ...
  </appSettings>
```

#### OAuth

TBC.

### Backoffice usage

TBC.

### Front-end rendering

TBC.