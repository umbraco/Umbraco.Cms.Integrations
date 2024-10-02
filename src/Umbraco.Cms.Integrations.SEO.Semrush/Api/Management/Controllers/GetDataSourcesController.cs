using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class GetDataSourcesController : SemrushControllerBase
    {
        public GetDataSourcesController(IOptions<SemrushSettings> options, IWebHostEnvironment webHostEnvironment, ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder, SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory)
        {
        }

        [HttpGet("datasources")]
        [ProducesResponseType(typeof(DataSourceDto), StatusCodes.Status200OK)]
        public IActionResult GetDataSources()
        {
            string semrushDataSourcesPath = $"{_webHostEnvironment.ContentRootPath}/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushDataSources.json";

            _lock.EnterReadLock();

            try
            {
                if (!System.IO.File.Exists(semrushDataSourcesPath))
                {
                    var fs = System.IO.File.Create(semrushDataSourcesPath);
                    fs.Close();

                    return Ok(new DataSourceDto());
                }

                var content = System.IO.File.ReadAllText(semrushDataSourcesPath);
                var dataSourceDto = new DataSourceDto
                {
                    Items = JsonConvert.DeserializeObject<List<DataSourceItemDto>>(content).Select(p =>
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
