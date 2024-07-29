﻿
namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services
{
    public interface ITokenService
    {
        bool TryGetParameters(string key, out string value);

        void SaveParameters(string key, string serializedParams);

        void RemoveParameters(string key);
    }
}
