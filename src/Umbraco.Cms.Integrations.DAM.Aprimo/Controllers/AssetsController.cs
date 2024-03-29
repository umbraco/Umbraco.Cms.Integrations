﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Dynamic;
using System.Text;
using System.Text.Json;

using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;
using Umbraco.Cms.Integrations.DAM.Aprimo.Extensions;
using Umbraco.Cms.Integrations.DAM.Aprimo.Migrations;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Controllers
{
    [PluginController("UmbracoCmsIntegrationsDamAprimo")]
    public class AssetsController : UmbracoAuthorizedApiController
    {
        private readonly AprimoSettings _settings;

        private readonly AprimoOAuthSettings _oauthSettings;

        private readonly IAprimoAuthorizationService _authorizationService;

        private readonly IAprimoService _assetsService;

        private readonly OAuthConfigurationStorage _oauthConfigurationStorage;

        public AssetsController(
            IOptions<AprimoSettings> options, 
            IOptions<AprimoOAuthSettings> oauthOptions,
            IAprimoAuthorizationService authorizationService,
            IAprimoService assetsService,
            OAuthConfigurationStorage oauthConfigurationStorage)
        {
            _settings = options.Value;

            _oauthSettings = oauthOptions.Value;

            _authorizationService= authorizationService;

            _assetsService = assetsService;

            _oauthConfigurationStorage = oauthConfigurationStorage;
        }

        [HttpGet]
        public async Task<IActionResult> CheckApiConfiguration()
        {
            if (string.IsNullOrEmpty(_settings.Tenant)
                || string.IsNullOrEmpty(_oauthSettings.ClientId)
                || string.IsNullOrEmpty(_oauthSettings.RedirectUri))
                return new JsonResult(AprimoResponse<string>.Fail(Constants.ErrorResources.InvalidApiConfiguration, false));

            var result = await _assetsService.GetRecordByIdAsync(Guid.NewGuid());
            if (!result.IsAuthorized)
            {
                await _authorizationService.RefreshAccessTokenAsync();

                var updatedResult = await _assetsService.GetRecordByIdAsync(Guid.NewGuid());

                return updatedResult.IsAuthorized
                    ? new JsonResult(AprimoResponse<string>.Ok(string.Empty))
                    : new JsonResult(AprimoResponse<string>.Fail(Constants.ErrorResources.Unauthorized, false));
            }

            return new JsonResult(AprimoResponse<string>.Ok(string.Empty));
        }

        [HttpGet]
        public string GetAuthorizationUrl()
        {
            // remove any existing record of code exchange
            _oauthConfigurationStorage.Delete();

            // generate new code exchange
            var oauthCodeExchange = OAuthHelper.GenerateKeys();

            var configurationEntity = new AprimoOAuthConfiguration
            {
                AccessToken = string.Empty,
                RefreshToken= string.Empty,
                CodeVerifier = oauthCodeExchange.CodeVerifier,
                CodeChallenge = oauthCodeExchange.CodeChallenge
            };
            _oauthConfigurationStorage.AddOrUpdate(configurationEntity);

            return _authorizationService.GetAuthorizationUrl(oauthCodeExchange);
        }

        [HttpGet]
        public string GetContentSelectorUrl()
        {
            dynamic selectorOptions = new ExpandoObject();
            selectorOptions.title = "Select media";
            selectorOptions.select = "single";

            var encodedOptions = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(selectorOptions)));

            return string.Format("https://{0}.dam.aprimo.com/dam/selectcontent#options={1}",
                _settings.Tenant, encodedOptions);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequest request) =>
            await _authorizationService.GetAccessTokenAsync(request.Code);

        [HttpGet]
        public async Task<IActionResult> GetRecordDetails(string id)
        {
            var record = await _assetsService.GetRecordByIdAsync(Guid.Parse(id));

            return new JsonResult(record);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecords(string page, string searchTerm)
        {
            var response = await _assetsService.SearchRecordsAsync(page, searchTerm);

            return new JsonResult(response);
        }

        [HttpDelete]
        public void RevokeAccessToken() => _oauthConfigurationStorage.Delete();
    }
}
