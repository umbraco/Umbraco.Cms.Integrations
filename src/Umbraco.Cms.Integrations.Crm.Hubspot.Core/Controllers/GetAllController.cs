using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Resources;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class GetAllController : HubSpotFormsControllerBase
{
    private readonly ILogger<GetAllController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HubspotSettings _settings;

    public GetAllController(
        ILogger<GetAllController> logger, 
        ITokenService tokenService, 
        IHttpClientFactory httpClientFactory,
        IOptions<HubspotSettings> options)
    {
        _logger = logger;
        _tokenService = tokenService;
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }

    [HttpGet("get-all")]
    public async Task<ResponseDto> GetAll()
    {
        var client = _httpClientFactory.CreateClient();

        var hubspotApiKey = _settings.ApiKey;

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
                Forms = ParseForms(forms, _settings.Region).ToList()
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
