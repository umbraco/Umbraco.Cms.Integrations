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
    public class GetColumnsController : SemrushControllerBase
    {
        public GetColumnsController(
            IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
        }

        [HttpGet("columns")]
        [ProducesResponseType(typeof(IEnumerable<ColumnDto>), StatusCodes.Status200OK)]
        public IActionResult GetColumns()
        {
            string semrushColumnsPath = "semrushColumns.json";

            _lock.EnterReadLock();

            try
            {
                if (!System.IO.File.Exists(semrushColumnsPath))
                {
                    var fs = System.IO.File.Create(semrushColumnsPath);
                    fs.Close();

                    return Ok(Enumerable.Empty<ColumnDto>());
                }

                var content = System.IO.File.ReadAllText(semrushColumnsPath);
                var deserializeContent = JsonSerializer.Deserialize<IEnumerable<ColumnDto>>(content).Select(p =>
                    new ColumnDto
                    {
                        Name = p.Name,
                        Value = p.Value,
                        Description = p.Description
                    });

                return Ok(deserializeContent);

            }
            catch (FileNotFoundException ex)
            {
                return Ok(Enumerable.Empty<ColumnDto>());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
