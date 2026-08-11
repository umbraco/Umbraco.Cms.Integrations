using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.SqlSyntax;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Migrations
{
    /// <summary>
    /// Moves the Shopify product picker from string to JSON storage.
    /// </summary>
    /// <remarks>
    /// The property editor previously used the default string value type, which stores its value in
    /// <c>varcharValue</c>. It now declares <see cref="ValueTypes.Json"/>, so existing data types need their
    /// <see cref="IDataType.DatabaseType"/> corrected and their property data relocated to <c>textValue</c>,
    /// or previously saved selections would read back as null.
    /// </remarks>
    public class MigrateProductPickerToJsonStorage : AsyncMigrationBase
    {
        private readonly IDataTypeService _dataTypeService;

        public MigrateProductPickerToJsonStorage(IMigrationContext context, IDataTypeService dataTypeService)
            : base(context) => _dataTypeService = dataTypeService;

        protected override async Task MigrateAsync()
        {
            var dataTypes = await _dataTypeService.GetByEditorAliasAsync(Constants.PropertyEditors.ProductPickerAlias);

            foreach (var dataType in dataTypes)
            {
                if (dataType.DatabaseType == ValueStorageType.Ntext) continue;

                dataType.DatabaseType = ValueStorageType.Ntext;

                await _dataTypeService.UpdateAsync(dataType, Cms.Core.Constants.Security.SuperUserKey);
            }

            // Relocate the values that were written while the editor still used string storage. The
            // "varcharValue IS NOT NULL" guard keeps this idempotent. Copying can be slow on a large
            // umbracoPropertyData table, so allow more than the default command timeout.
            // This is what MigrationBase.EnsureLongCommandTimeout does; it is inlined here because
            // that helper only arrived in Umbraco 17.4 and this package supports 17.0 upwards.
            const int longCommandTimeoutSeconds = 300;
            if (Database.CommandTimeout < longCommandTimeoutSeconds)
            {
                Database.CommandTimeout = longCommandTimeoutSeconds;
            }

            await Database.ExecuteAsync(BuildRelocateSql(Database));

            // Saves made while the bug was live wrote the collection's ToString() instead of the ids.
            // Those ids cannot be recovered, but the text has to go: the value converter deserializes
            // it as JSON and throws, taking the whole page down until an editor re-picks the products.
            await Database.ExecuteAsync(BuildClearBrokenValuesSql(Database));
        }

        // Umbraco's own Dto classes carry these names but are internal to Umbraco.Infrastructure,
        // so a package has to spell the columns out. The table names are public constants.
        private const string TextValue = "textValue";
        private const string VarcharValue = "varcharValue";
        private const string PropertyTypeId = "propertyTypeId";
        private const string DataTypeId = "dataTypeId";
        private const string EditorAlias = "propertyEditorAlias";
        private const string DbType = "dbType";
        private const string PropertyTypePrimaryKey = "id";
        private const string DataTypeNodeId = "nodeId";

        /// <summary>
        /// Matches every property that uses a product picker data type now stored as Ntext.
        /// </summary>
        private static string ProductPickerProperties(ISqlSyntaxProvider syntax) => $@"
{syntax.GetQuotedColumnName(PropertyTypeId)} IN (
    SELECT {syntax.GetQuotedColumnName(PropertyTypePrimaryKey)}
    FROM {syntax.GetQuotedTableName(Cms.Core.Constants.DatabaseSchema.Tables.PropertyType)}
    WHERE {syntax.GetQuotedColumnName(DataTypeId)} IN (
        SELECT {syntax.GetQuotedColumnName(DataTypeNodeId)}
        FROM {syntax.GetQuotedTableName(Cms.Core.Constants.DatabaseSchema.Tables.DataType)}
        WHERE {syntax.GetQuotedColumnName(EditorAlias)} = '{Constants.PropertyEditors.ProductPickerAlias}'
        AND {syntax.GetQuotedColumnName(DbType)} = '{nameof(ValueStorageType.Ntext)}'
    )
)";

        private static string BuildRelocateSql(IUmbracoDatabase database)
        {
            ISqlSyntaxProvider syntax = database.SqlContext.SqlSyntax;

            return $@"
UPDATE {syntax.GetQuotedTableName(Cms.Core.Constants.DatabaseSchema.Tables.PropertyData)}
SET {syntax.GetQuotedColumnName(TextValue)} = {syntax.GetQuotedColumnName(VarcharValue)}, {syntax.GetQuotedColumnName(VarcharValue)} = NULL
WHERE {ProductPickerProperties(syntax)}
AND {syntax.GetQuotedColumnName(VarcharValue)} IS NOT NULL";
        }

        private static string BuildClearBrokenValuesSql(IUmbracoDatabase database)
        {
            ISqlSyntaxProvider syntax = database.SqlContext.SqlSyntax;

            return $@"
UPDATE {syntax.GetQuotedTableName(Cms.Core.Constants.DatabaseSchema.Tables.PropertyData)}
SET {syntax.GetQuotedColumnName(TextValue)} = NULL
WHERE {ProductPickerProperties(syntax)}
AND {syntax.GetQuotedColumnName(TextValue)} LIKE 'System.Collections.%'";
        }
    }
}
