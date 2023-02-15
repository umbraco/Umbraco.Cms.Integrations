
namespace Umbraco.Cms.Integrations.Library.Parsing
{
    public class EnumerableParser : BaseParser<IEnumerable<object>>
    {
        public override string Parse(IEnumerable<object> input) => string.Join(",", input.Select(p => p.ToString()));

        public override string Parse(List<IEnumerable<object>> input)
        {
            throw new NotImplementedException();
        }
    }
}
