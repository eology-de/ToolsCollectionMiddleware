using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace eology.ToolsCollection;

[Dependency(ReplaceServices = true)]
public class ToolsCollectionBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "ToolsCollection";
}
