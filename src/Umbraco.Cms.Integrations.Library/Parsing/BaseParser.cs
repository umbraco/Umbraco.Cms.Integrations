
namespace Umbraco.Cms.Integrations.Library.Parsing
{
    public abstract class BaseParser<T> where T : class
    {
        public abstract string Parse(T input);

        public abstract string Parse(List<T> input);    
    }
}
