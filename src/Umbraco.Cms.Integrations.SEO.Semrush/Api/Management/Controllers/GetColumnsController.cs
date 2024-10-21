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
            string semrushColumnsPath = $"{Constants.EmbeddedResourceNamespace}.semrushColumns.json";
            var assembly = Assembly.GetExecutingAssembly();

            _lock.EnterReadLock();

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(semrushColumnsPath))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();
                            var deserializeContent = JsonSerializer.Deserialize<IEnumerable<ColumnDto>>(result).Select(p =>
                            new ColumnDto
                            {
                                Name = p.Name,
                                Value = p.Value,
                                Description = p.Description
                            });

                            return Ok(deserializeContent);
                        }
                    }
                    else
                    {
                        return Ok(Enumerable.Empty<ColumnDto>());
                    }
                }

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
