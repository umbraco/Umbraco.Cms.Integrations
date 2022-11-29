# Umbraco.Forms.Integrations.Search.Algolia

This integration provides a custom dashboard for managing search indices in Algolia.

## Prerequisites

Required minimum versions of Umbraco CMS: 
- CMS: 10.1.0
- Algolia.Search: 6.13.0

## How To Use

### Authentication

The communication with Algolia is handled through their [.NET API client](https://www.algolia.com/doc/api-client/getting-started/install/csharp/?client=csharp), 
which requires an Application ID and an API key. 
They are used to initialize the [`SearchClient`](https://github.com/algolia/algoliasearch-client-csharp/blob/master/src/Algolia.Search/Clients/SearchClient.cs)
which handles indexing and searching operations.

### Configuration

The following configuration is required for working with the Algolia API:

```
{
  "Umbraco": {
    "CMS": {
      "Integrations": {
        "Search": {
          "Algolia": {
            "Settings": {
              "ApplicationId": "[your_application_id]",
              "AdminApiKey": "[your_admin_api_key]]"
            }
          }
        }
      }
    }
  }
}
```

### Working with the Umbraco CMS - Algolia integration

In the backoffice, go to the _Settings_ section and look for the _Algolia Search Management_ dashboard.

In this view you will be able to create definitions for indices in Algolia, by providing a name for the index and 
selecting the document types you want indexed, and which fields of the document types you want to include.

After creating an index, only the content definition is saved into the _algoliaIndices_ table in Umbraco and an empty 
index is created in Algolia. 

The actual content payload is pushed to Algolia for indices created in Umbraco on two scenarios:
- from the list of indices, the _Build_ action is triggered, resulting in all content of specific document types to be sent as JSON to Algolia.
- using a _ContentPublishedNotification_ handler which will check the list of indices for the specific document type, and will update a matching Algolia object.

From the dashboard you can also perform a search over one selected index, or remove it.

Two additional handlers for _ContentDeletedNotification_ and _ContentUnpublishedNotification_ will remove the matching object from Algolia.

Each Umbraco content item indexed in Algolia is referenced by the content entity's `GUID` Key field.
