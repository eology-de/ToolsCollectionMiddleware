using eology.ToolsCollection.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace eology.ToolsCollection.Permissions;

public class ToolsCollectionPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var ToolsCollectionGroup = context.AddGroup(ToolsCollectionPermissions.GroupName);

        var serpPermission = ToolsCollectionGroup.AddPermission(ToolsCollectionPermissions.Serps.Default, L("Permission:Serps"));
        serpPermission.AddChild(ToolsCollectionPermissions.Serps.Create, L("Permission:Serps.Create"));
        serpPermission.AddChild(ToolsCollectionPermissions.Serps.Edit, L("Permission:Serps.Edit"));
        serpPermission.AddChild(ToolsCollectionPermissions.Serps.Delete, L("Permission:Serps.Delete"));

        ToolsCollectionGroup.AddPermission(ToolsCollectionPermissions.Hangfire.Default, L("Permission:Hangfire"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ToolsCollectionResource>(name);
    }
}
