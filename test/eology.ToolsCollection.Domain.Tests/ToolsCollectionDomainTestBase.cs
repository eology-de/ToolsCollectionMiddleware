using Volo.Abp.Modularity;

namespace eology.ToolsCollection;

/* Inherit from this class for your domain layer tests. */
public abstract class ToolsCollectionDomainTestBase<TStartupModule> : ToolsCollectionTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
