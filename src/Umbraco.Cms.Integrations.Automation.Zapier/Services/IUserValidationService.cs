using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public interface IUserValidationService
    {
        Task<bool> Validate(string username, string password, string apiKey);
    }
}
