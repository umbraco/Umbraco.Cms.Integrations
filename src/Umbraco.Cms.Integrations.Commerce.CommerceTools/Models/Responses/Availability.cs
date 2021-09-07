using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Responses
{
    internal class Availability
    {
        public int AvailableQuantity { get; set; }

        public bool IsOnStock { get; set; }
    }
}
