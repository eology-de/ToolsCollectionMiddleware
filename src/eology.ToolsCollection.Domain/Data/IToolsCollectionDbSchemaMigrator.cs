using System.Threading.Tasks;

namespace eology.ToolsCollection.Data;

public interface IToolsCollectionDbSchemaMigrator
{
    Task MigrateAsync();
}
