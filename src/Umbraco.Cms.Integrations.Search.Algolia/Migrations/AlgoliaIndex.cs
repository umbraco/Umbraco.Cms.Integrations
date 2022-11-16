using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Cms.Integrations.Search.Algolia.Migrations
{
    [TableName(Constants.AlgoliaIndicesTableName)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class AlgoliaIndex
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("SerializedData")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string SerializedData { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }
    }
}
