using System;
using System.Collections.Generic;

using Volo.Abp.Domain.Entities.Auditing;

namespace eology.ToolsCollection.Features
{
    public class SerpItem : AuditedAggregateRoot<Guid>
    {
        public string RequestName { get; set; }

        public virtual ICollection<SerpQueueItem> QueueItems { get; set; }

        public int ResultCount { get; set; }

        public SerpItem()
        {
            QueueItems = new List<SerpQueueItem>();
            RequestName = "";
        }

        public SerpItem(Guid id, string requestName) : base(id)
        {
            RequestName = requestName;
            QueueItems = new List<SerpQueueItem>();
        }
    }
}
