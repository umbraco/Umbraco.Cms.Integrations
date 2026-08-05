using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Xunit;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Tests.Controllers;

public class CheckConfigurationControllerTests
{
    private static HubspotFormPickerSettings Act(
        Microsoft.Extensions.Options.IOptions<Configuration.HubspotSettings> settings,
        Microsoft.Extensions.Options.IOptions<Configuration.HubspotOAuthSettings> oauthSettings)
    {
        var sut = new CheckConfigurationController(settings, oauthSettings);

        var result = sut.CheckConfiguration();

        var ok = Assert.IsType<OkObjectResult>(result);
        return Assert.IsType<HubspotFormPickerSettings>(ok.Value);
    }

    [Fact]
    public void CheckConfiguration_WithApiKey_ReportsValidApiConfiguration()
    {
        var settings = Act(TestHelpers.ApiSettings(), TestHelpers.EmptyOAuthSettings());

        Assert.True(settings.IsValid);
        Assert.Equal(ConfigurationType.Api.Value, settings.Type.Value);
    }

    [Fact]
    public void CheckConfiguration_WithoutApiKeyButUsingUmbracoAuthorization_ReportsValidOAuthConfiguration()
    {
        // UseUmbracoAuthorization defaults to true, so no explicit OAuth settings are needed.
        var settings = Act(TestHelpers.OAuthSettings(), TestHelpers.EmptyOAuthSettings());

        Assert.True(settings.IsValid);
        Assert.Equal(ConfigurationType.OAuth.Value, settings.Type.Value);
    }

    [Fact]
    public void CheckConfiguration_WithCompleteOwnOAuthSettings_ReportsValidOAuthConfiguration()
    {
        var settings = Act(TestHelpers.NoAuthorizationSettings(), TestHelpers.CompleteOAuthSettings());

        Assert.True(settings.IsValid);
        Assert.Equal(ConfigurationType.OAuth.Value, settings.Type.Value);
    }

    [Fact]
    public void CheckConfiguration_WithNoConfigurationAtAll_ReportsInvalid()
    {
        var settings = Act(TestHelpers.NoAuthorizationSettings(), TestHelpers.EmptyOAuthSettings());

        Assert.False(settings.IsValid);
        Assert.Equal(ConfigurationType.None.Value, settings.Type.Value);
    }

    [Fact]
    public void CheckConfiguration_WithIncompleteOwnOAuthSettings_ReportsInvalid()
    {
        // ClientId alone is not enough - scopes, secret and token endpoint are all required.
        var partial = Microsoft.Extensions.Options.Options.Create(
            new Configuration.HubspotOAuthSettings { ClientId = "client-id" });

        var settings = Act(TestHelpers.NoAuthorizationSettings(), partial);

        Assert.False(settings.IsValid);
        Assert.Equal(ConfigurationType.None.Value, settings.Type.Value);
    }
}
