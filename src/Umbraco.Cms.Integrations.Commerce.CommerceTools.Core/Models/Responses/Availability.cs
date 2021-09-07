using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class Availability
    {
        public int AvailableQuantity { get; set; }

        public bool IsOnStock { get; set; }
    }
}
