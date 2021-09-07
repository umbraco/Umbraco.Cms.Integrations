using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Responses
{
    internal class MasterData
    {
        public bool Published { get; set; }

        public ProductData Current { get; set; }
    }
}
