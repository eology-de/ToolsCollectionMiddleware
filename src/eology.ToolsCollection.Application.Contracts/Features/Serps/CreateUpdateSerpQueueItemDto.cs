using System;
using Volo.Abp.Application.Dtos;

namespace eology.ToolsCollection.Features.Serps
{
    public class CreateUpdateSerpQueueItemDto : EntityDto<Guid>
    {
        public string Query { get; set; }
        public long TaskId { get; set; }
        public string QueryResultSerialized { get; set; }

        public CreateUpdateSerpQueueItemDto()
        {
            Query = "";
            QueryResultSerialized = "";
        }
    }
}
