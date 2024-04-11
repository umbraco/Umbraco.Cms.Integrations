using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class ValidateAccessTokenController : HubSpotFormsControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;

    public ValidateAccessTokenController(IHttpClientFactory httpClientFactory, ITokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
    }

    [HttpGet("validate")]
    public async Task<ResponseDto> ValidateAccessToken()
    {
        var client = _httpClientFactory.CreateClient();

        _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);

        if (string.IsNullOrEmpty(accessToken))
            return new ResponseDto
            {
                IsValid = false,
                Error = Constants.ErrorMessages.OAuthInvalidToken
            };

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(HubspotFormsApiEndpoint)
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(requestMessage);

        return new ResponseDto
        {
            IsValid = response.IsSuccessStatusCode,
            IsExpired = response.StatusCode == HttpStatusCode.Unauthorized,
            Error = !response.IsSuccessStatusCode ? Constants.ErrorMessages.OAuthInvalidToken : string.Empty
        };
    }
}
