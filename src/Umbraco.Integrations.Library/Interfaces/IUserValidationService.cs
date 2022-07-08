using System.Threading.Tasks;

namespace Umbraco.Integrations.Library.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> Validate(string username, string password, string apiKey, string userGroup = "");
    }
}
