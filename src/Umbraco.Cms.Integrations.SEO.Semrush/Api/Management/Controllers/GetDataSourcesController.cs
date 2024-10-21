using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class GetDataSourcesController : SemrushControllerBase
    {
        public GetDataSourcesController(IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
        }

        [HttpGet("data-sources")]
        [ProducesResponseType(typeof(DataSourceDto), StatusCodes.Status200OK)]
        public IActionResult GetDataSources()
        {
            string semrushDataSourcesPath = $"{Constants.EmbeddedResourceNamespace}.semrushDataSources.json";
            var assembly = Assembly.GetExecutingAssembly();

            _lock.EnterReadLock();
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(semrushDataSourcesPath))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();
                            var dataSourceDto = new DataSourceDto
                            {
                                Items = JsonSerializer.Deserialize<List<DataSourceItemDto>>(result).Select(p =>
                                    new DataSourceItemDto
                                    {
                                        Code = p.Code,
                                        Region = p.Region,
                                        ResearchTypes = p.ResearchTypes,
                                        GoogleSearchDomain = p.GoogleSearchDomain
                                    })
                            };

                            return Ok(dataSourceDto);
                        }
                    }
                    else 
                    {
                        return Ok(new DataSourceDto());
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                return Ok(new DataSourceDto());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
