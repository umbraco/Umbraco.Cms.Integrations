using Algolia.Search.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers.ManagementApi
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "AlgoliaSearch")]
    [Route($"{Constants.ManagementApi.RootPath}/v1/search/")]
    public abstract class SearchControllerBase : AlgoliaManagementApiControllerBase
    {
        protected IAlgoliaIndexService IndexService { get; }

        protected IAlgoliaSearchService<SearchResponse<Record>> SearchService { get; }

        protected IAlgoliaIndexDefinitionStorage<AlgoliaIndex> IndexStorage { get; }

        protected IUserService UserService {  get; }

        protected IPublishedUrlProvider UrlProvider {  get; }

        protected IContentService ContentService {  get; }

        protected IAlgoliaSearchPropertyIndexValueFactory AlgoliaSearchPropertyIndexValueFactory { get; }

        protected IUmbracoContextFactory UmbracoContextFactory { get; }

        protected IRecordBuilderFactory RecordBuilderFactory { get; }

        protected ILogger Logger { get; }

        public SearchControllerBase(
            IAlgoliaIndexService indexService,
            IAlgoliaSearchService<SearchResponse<Record>> searchService,
            IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
            IUserService userService,
            IPublishedUrlProvider urlProvider,
            IContentService contentService,
            IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger logger,
            IRecordBuilderFactory recordBuilderFactory
            )
        {
            IndexService = indexService;

            SearchService = searchService;

            IndexStorage = indexStorage;

            UserService = userService;

            UrlProvider = urlProvider;

            ContentService = contentService;

            AlgoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;

            UmbracoContextFactory = umbracoContextFactory;

            Logger = logger;

            RecordBuilderFactory = recordBuilderFactory;
        }

    }
}
