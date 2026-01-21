using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GoogleSearchConsoleGroupName)]
public class InspectController : GoogleControllerBase
{
    public InspectController(IOptions<GoogleSearchConsoleSettings> options, ITokenService tokenService, IHttpClientFactory clientFactory, GoogleComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
        : base(options, tokenService, clientFactory, authorizationImplementationFactory)
    {
    }

    [HttpPost("inspect")]
    [ProducesResponseType(typeof(InspectionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Inspect([FromBody] UrlInspectionDto dto)
    {
        TokenService.TryGetParameters(Constants.TokenDbKey, out string accessToken);
       
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Settings.InspectUrl),
            Content = new StringContent(JsonSerializer.Serialize(dto))
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var client = ClientFactory.CreateClient();

        var response = await client.SendAsync(requestMessage);

        // If access token has expired or is invalid, attempt to retrieve a new one using the refresh token.
        // Then, retry the request once.
        // If response is Unauthorize, notify the user to re-authenticate.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var result = await AuthorizationService.RefreshAccessTokenAsync();
            if (result.Contains("error"))
            {
                return BadRequest(
                    new ProblemDetailsBuilder()
                    .WithTitle("Unable to refresh access token. Please re-authenticate.")
                    .Build());
            }

            TokenService.TryGetParameters(Constants.TokenDbKey, out string refreshedAccessToken);
            requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Settings.InspectUrl),
                Content = new StringContent(JsonSerializer.Serialize(dto))
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedAccessToken);
            response = await client.SendAsync(requestMessage);
        }

        var content = await response.Content.ReadAsStringAsync();

        var responseDto = JsonSerializer.Deserialize<ResponseDto>(content);

        return responseDto!.Error is not null
            ? BadRequest(new ProblemDetailsBuilder().WithTitle(responseDto.Error.Message).Build()) 
            : Ok(responseDto.InspectionResult);
    }
}
