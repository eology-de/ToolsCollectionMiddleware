using eology.ToolsCollection.Samples;
using Xunit;

namespace eology.ToolsCollection.EntityFrameworkCore.Applications;

[Collection(ToolsCollectionTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ToolsCollectionEntityFrameworkCoreTestModule>
{

}
