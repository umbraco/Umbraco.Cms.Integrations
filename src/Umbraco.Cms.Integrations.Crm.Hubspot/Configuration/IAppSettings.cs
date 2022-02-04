using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public interface IAppSettings
    {
        string this[string key] { get; }
    }
}
