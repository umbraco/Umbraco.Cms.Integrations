using System;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Helpers
{
    public class ReflectionHelper
    {
        public static bool IsFormsExtensionInstalled => Type.GetType("Umbraco.Forms.Core.Services.IFormService, Umbraco.Forms.Core") != null;
    }
}
