﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class RevokeAccessTokenController : FormsControllerBase
    {
        public RevokeAccessTokenController(IOptions<DynamicsSettings> options, DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService, DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, dynamicsService, dynamicsConfigurationService, authorizationImplementationFactory)
        {
        }

        [HttpDelete("revoke-access-token")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult RevokeAccessToken() => Ok(DynamicsConfigurationService.Delete());
    }
}