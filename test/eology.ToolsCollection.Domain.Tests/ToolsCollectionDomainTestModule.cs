using Volo.Abp.Modularity;

namespace eology.ToolsCollection;

[DependsOn(
    typeof(ToolsCollectionDomainModule),
    typeof(ToolsCollectionTestBaseModule)
)]
public class ToolsCollectionDomainTestModule : AbpModule
{

}
