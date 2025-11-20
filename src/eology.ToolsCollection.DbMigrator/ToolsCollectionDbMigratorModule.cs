using eology.ToolsCollection.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace eology.ToolsCollection.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ToolsCollectionEntityFrameworkCoreModule),
    typeof(ToolsCollectionApplicationContractsModule)
    )]
public class ToolsCollectionDbMigratorModule : AbpModule
{
}
