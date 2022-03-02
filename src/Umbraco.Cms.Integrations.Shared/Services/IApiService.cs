using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Shared.Models;
using Umbraco.Cms.Integrations.Shared.Models.Dtos;

namespace Umbraco.Cms.Integrations.Shared.Services
{
    public interface IApiService<T> where T : class
    {
        EditorSettings GetApiConfiguration();

        string GetAuthorizationUrl();

        Task<string> GetAccessToken(OAuthRequestDto request);

        Task<ResponseDto<T>> ValidateAccessToken();

        void RevokeAccessToken();

        Task<ResponseDto<T>> GetResults();
    }
}
