using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace eology.ToolsCollection.Data;

/* This is used if database provider does't define
 * IToolsCollectionDbSchemaMigrator implementation.
 */
public class NullToolsCollectionDbSchemaMigrator : IToolsCollectionDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
