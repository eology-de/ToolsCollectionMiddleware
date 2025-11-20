using System;
using Volo.Abp.Application.Dtos;

namespace eology.ToolsCollection.Features.Serps
{
    public class SerpItemPagedSortedFilteredRequestDto : PagedAndSortedResultRequestDto
    {
        public string? RequestName { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? Id { get; set; }
    }
}

