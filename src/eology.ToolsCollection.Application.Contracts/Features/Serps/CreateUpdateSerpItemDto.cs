using System;
using Volo.Abp.Application.Dtos;

namespace eology.ToolsCollection.Features.Serps
{
    public class CreateUpdateSerpItemDto : EntityDto<Guid>
    {
        public string RequestName { get; set; }
        public string? Keywords { get; set; }
        public int ResultCount { get; set; }

        public CreateUpdateSerpItemDto()
        {
            RequestName = "";
        }
    }
}
