using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Resources;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetAllController : HubspotFormsControllerBase
    {
        private readonly ILogger<GetAllController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetAllController(
            IOptions<HubspotSettings> settingsOptions,
            ILogger<GetAllController> logger,
            ITokenService tokenService,
            IHttpClientFactory httpClientFactory) 
            : base(settingsOptions)
        {
            _logger = logger;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("get")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        public async Task<ResponseDto> GetAll()
        {
            var client = _httpClientFactory.CreateClient();

            var hubspotApiKey = Settings.ApiKey;

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
                _logger.LogInformation(message: Constants.ErrorMessages.ApiKeyMissing);

                return new ResponseDto { IsValid = false, Error = Constants.ErrorMessages.ApiKeyMissing };
            }

            string responseContent = string.Empty;
            try
            {
                var requestMessage = CreateRequest(hubspotApiKey);

                var response = await client.SendAsync(requestMessage);

                responseContent = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                var forms = await response.Content.ReadAsStringAsync();

                return new ResponseDto
                {
                    IsValid = true,
                    Forms = ParseForms(forms, Settings.Region).ToList()
                };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Forbidden.ToString()))
            {
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto { IsValid = false, Error = Constants.ErrorMessages.TokenPermissions };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
            }
            catch
            {
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto();
            }
        }
    }
}
