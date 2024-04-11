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

public class GetAllOAuthController : HubSpotFormsControllerBase
{
    private readonly ILogger<GetAllOAuthController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HubspotSettings _settings;

    public GetAllOAuthController(
        ILogger<GetAllOAuthController> logger, 
        ITokenService tokenService, 
        IHttpClientFactory httpClientFactory,
        IOptions<HubspotSettings> options)
    {
        _logger = logger;
        _tokenService = tokenService;
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }

    [HttpGet("get-all-oauth")]
    public async Task<ResponseDto> GetAllOAuth()
    {
        var client = _httpClientFactory.CreateClient();

        _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogInformation(Constants.ErrorMessages.AccessTokenMissing);

            return new ResponseDto
            {
                IsValid = false,
                Error = Constants.ErrorMessages.OAuthFetchFormsConfigurationFailed
            };
        }

        string responseContent = string.Empty;
        try
        {
            var requestMessage = CreateRequest(accessToken);

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
        catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
        {
            _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

            return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
        }
        catch
        {
            _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

            return new ResponseDto();
        }
    }
}
