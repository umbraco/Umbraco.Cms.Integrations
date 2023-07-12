using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Configuration
{
    public abstract class AppSettings
    {
        public string UserGroupAlias { get; set; }

        public string ApiKey { get; set; }
    }
}
