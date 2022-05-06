﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Helpers
{
    public class TriggerHelper
    {
        private readonly ZapierService _zapierService;

        public TriggerHelper(ZapierService zapierService)
        {
            _zapierService = zapierService;
        }

        public string Execute(string hookUrl, string contentId, string contentName)
        {
            var content = new Dictionary<string, string>
            {
                {Constants.Content.Id, contentId},
                {Constants.Content.Name, contentName},
                {Constants.Content.PublishDate, DateTime.UtcNow.ToString()}
            };

            var t = Task.Run(
                async () => await _zapierService.TriggerAsync(hookUrl, content));

            return t.Result;
        }

        public string FormExecute(string hookUrl, string formId, string formName, string pageUrl)
        {
            var content = new Dictionary<string, string>
            {
                {Constants.Form.Id, formId},
                {Constants.Form.Name, formName},
                {Constants.Form.SubmissionDate, DateTime.UtcNow.ToString()},
                {Constants.Form.PageUrl, pageUrl}
            };

            var t = Task.Run(
                async () => await _zapierService.TriggerAsync(hookUrl, content));

            return t.Result;
        }
    }
}