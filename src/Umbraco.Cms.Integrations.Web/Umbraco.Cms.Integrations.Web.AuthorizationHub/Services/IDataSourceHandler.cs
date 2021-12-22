
namespace Umbraco.Cms.Integrations.Web.AuthorizationHub.Services
{
    public interface IDataSourceHandler
    {
        IDataSourceHandler Build(string dir);

        void Write(string path, string content);

        string Read(string path);
    }
}
