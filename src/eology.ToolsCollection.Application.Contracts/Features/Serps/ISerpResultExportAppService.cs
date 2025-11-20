using System;
using Volo.Abp.Application.Services;

namespace eology.ToolsCollection.Features.Serps
{
    public interface ISerpResultExportAppService :
    IReadOnlyAppService<
        SerpItemDto,
        Guid,
        SerpItemPagedSortedFilteredRequestDto
        >
    {

    }
}
