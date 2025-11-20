using Volo.Abp.Modularity;

namespace eology.ToolsCollection;

[DependsOn(
    typeof(ToolsCollectionApplicationModule),
    typeof(ToolsCollectionDomainTestModule)
)]
public class ToolsCollectionApplicationTestModule : AbpModule
{

}
