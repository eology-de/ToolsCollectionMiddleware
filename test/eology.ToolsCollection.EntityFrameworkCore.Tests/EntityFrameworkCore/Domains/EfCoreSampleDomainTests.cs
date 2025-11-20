using eology.ToolsCollection.Samples;
using Xunit;

namespace eology.ToolsCollection.EntityFrameworkCore.Domains;

[Collection(ToolsCollectionTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ToolsCollectionEntityFrameworkCoreTestModule>
{

}
