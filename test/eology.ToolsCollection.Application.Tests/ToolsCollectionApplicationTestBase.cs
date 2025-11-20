using Volo.Abp.Modularity;

namespace eology.ToolsCollection;

public abstract class ToolsCollectionApplicationTestBase<TStartupModule> : ToolsCollectionTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
