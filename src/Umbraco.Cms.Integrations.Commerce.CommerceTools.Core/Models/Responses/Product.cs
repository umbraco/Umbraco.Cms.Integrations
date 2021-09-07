using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class Product
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public MasterData MasterData { get; set; }
    }
}
