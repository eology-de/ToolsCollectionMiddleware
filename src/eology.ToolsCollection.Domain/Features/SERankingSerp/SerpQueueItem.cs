using System;
using System.ComponentModel.DataAnnotations.Schema;
using eology.ToolsCollection.Features.Serps;
using Volo.Abp.Domain.Entities.Auditing;

namespace eology.ToolsCollection.Features
{
    public class SerpQueueItem : AuditedAggregateRoot<Guid>
    {
        public Guid ParentId { get; set; } // Fremdschlüssel zu SerpRequestItem [6]
        public string Query { get; set; }
        public int TaskId { get; set; }
        public string? QueryResultSerialized { get; set; }
        public SerpQueueItemStateEnum Status { get; set; }

        [ForeignKey("ParentId")]
        public virtual SerpItem Parent { get; set; }

        public SerpQueueItem()
        {
            Query = "";
            QueryResultSerialized = "";
        }

        public SerpQueueItem( Guid parentId, string query, int taskId) : base()
        {
            ParentId = parentId;
            Query = query;
            TaskId = taskId;
        }

        public SerpQueueItem(Guid id, Guid parentId, string query, int taskId ) : base(id)
        {
            ParentId = parentId;
            Query = query;
            TaskId = taskId;
        }
    }
}

