using Volo.Abp.Settings;

namespace eology.ToolsCollection.Settings;

public class ToolsCollectionSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ToolsCollectionSettings.MySetting1));
    }
}
