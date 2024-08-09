﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using static Umbraco.Cms.Integrations.Crm.Dynamics.DynamicsComposer;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    [Route($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/forms")]
    public class FormsControllerBase : DynamicsControllerBase
    {
        protected readonly DynamicsSettings DynamicsSettings;

        protected readonly IDynamicsAuthorizationService AuthorizationService;

        protected readonly DynamicsService DynamicsService;

        protected readonly DynamicsConfigurationService DynamicsConfigurationService;

        public FormsControllerBase(IOptions<DynamicsSettings> options,
            DynamicsService dynamicsService,
            DynamicsConfigurationService dynamicsConfigurationService,
            AuthorizationImplementationFactory authorizationImplementationFactory)
        {

            DynamicsSettings = options.Value;

            AuthorizationService = authorizationImplementationFactory(DynamicsSettings.UseUmbracoAuthorization);

            DynamicsService = dynamicsService;

            DynamicsConfigurationService = dynamicsConfigurationService;
        }
    }
}
