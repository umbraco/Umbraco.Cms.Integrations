using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
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
    public async Task<IActionResult> Inspect([FromBody] UrlInspectionDto dto, CancellationToken cancellationToken = default)
    {
        var response = await SendInspectRequestAsync(dto, cancellationToken);

        // If access token has expired or is invalid, attempt to retrieve a new one using the refresh token.
        // Then, retry the request once.
        // If response is Unauthorize, notify the user to re-authenticate.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();

            var refreshResult = await AuthorizationService.RefreshAccessTokenAsync();
            if (!refreshResult.Success)
            {
                return BadRequest(
                    new ProblemDetailsBuilder()
                    .WithTitle("Unable to refresh access token. Please re-authenticate.")
                    .Build());
            }

            response = await SendInspectRequestAsync(dto, cancellationToken);
        }

        using (response)
        {
            var content = await response.Content.ReadAsStringAsync();

            var responseDto = JsonSerializer.Deserialize<ResponseDto>(content);

            if (responseDto?.Error is not null)
            {
                return BadRequest(
                    new ProblemDetailsBuilder()
                        .WithTitle(responseDto.Error.Message)
                        .Build());
            }

            return Ok(responseDto?.InspectionResult);
        }
    }

    private async Task<HttpResponseMessage> SendInspectRequestAsync(UrlInspectionDto dto, CancellationToken cancellationToken)
    {
        TokenService.TryGetParameters(Constants.TokenDbKey, out string accessToken);

        using var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Settings.InspectUrl),
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var client = ClientFactory.CreateClient();
        return await client.SendAsync(requestMessage, cancellationToken);
    }
}
