using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public interface IDynamicsConfigurationStorage
    {
        string AddOrUpdateOAuthConfiguration(string accessToken, string userId, string fullName);

        OAuthConfigurationDto GetOAuthConfiguration();

        string GetSystemUserFullName();

        string Delete();
    }
}
