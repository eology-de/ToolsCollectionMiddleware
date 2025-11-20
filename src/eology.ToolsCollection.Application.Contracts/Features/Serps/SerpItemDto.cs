using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace eology.ToolsCollection.Features.Serps
{
    public class SerpItemDto : AuditedEntityDto<Guid>
    {
        public string RequestName { get; set; }
        public string Keywords { get; set; }
        public int Total { get; set; }
        public int Part { get; set; }
        public List<SerpQueueItemDto> QueueItems{ get; set; }
        public int ResultCount { get; set; }

        public SerpItemDto()
        {
            RequestName = "";
            Keywords = "";
            QueueItems = new List<SerpQueueItemDto>();
        }
    }
}

