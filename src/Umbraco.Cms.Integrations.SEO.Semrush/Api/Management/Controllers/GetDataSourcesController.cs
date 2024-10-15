using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            string semrushDataSourcesPath = "semrushDataSources.json";

            _lock.EnterReadLock();
            try
            {
                if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), semrushDataSourcesPath)))
                {
                    var fs = System.IO.File.Create(semrushDataSourcesPath);
                    fs.Close();

                    return Ok(new DataSourceDto());
                }

                var content = System.IO.File.ReadAllText(semrushDataSourcesPath);
                var dataSourceDto = new DataSourceDto
                {
                    Items = JsonSerializer.Deserialize<List<DataSourceItemDto>>(content).Select(p =>
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
