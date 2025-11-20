using Xunit;

namespace eology.ToolsCollection.EntityFrameworkCore;

[CollectionDefinition(ToolsCollectionTestConsts.CollectionDefinitionName)]
public class ToolsCollectionEntityFrameworkCoreCollection : ICollectionFixture<ToolsCollectionEntityFrameworkCoreFixture>
{

}
