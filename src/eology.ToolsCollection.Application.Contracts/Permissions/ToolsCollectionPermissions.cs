namespace eology.ToolsCollection.Permissions;

public static class ToolsCollectionPermissions
{
    public const string GroupName = "ToolsCollection";

    public static class Serps
    {
        public const string Default = GroupName + ".Serps";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Hangfire
    {
        public const string Default = GroupName + ".Hangfire";
    }
}