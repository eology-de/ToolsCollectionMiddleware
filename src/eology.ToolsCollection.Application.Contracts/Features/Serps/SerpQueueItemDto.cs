using System;
using Volo.Abp.Application.Dtos;

namespace eology.ToolsCollection.Features.Serps
{
    public class SerpQueueItemDto : AuditedEntityDto<Guid>
    {
        public string Query { get; set; }
        public int TaskId { get; set; }
        public string QueryResultSerialized { get; set; }
        public SerpQueueItemStateEnum Status { get; set; }

        public SerpQueueItemDto()
        {
            Query = "";
            QueryResultSerialized = "";
        }
    }
}