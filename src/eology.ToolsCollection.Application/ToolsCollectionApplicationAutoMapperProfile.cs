using AutoMapper;
using eology.ToolsCollection.Features;
using eology.ToolsCollection.Features.Serps;

namespace eology.ToolsCollection;

public class ToolsCollectionApplicationAutoMapperProfile : Profile
{
    public ToolsCollectionApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<SerpItem, SerpItemDto>();
        CreateMap<SerpItemDto, SerpItem>();

        CreateMap<SerpItem, CreateUpdateSerpItemDto>();
        CreateMap<CreateUpdateSerpItemDto, SerpItem>();

        CreateMap<SerpQueueItem, SerpQueueItemDto>();
        CreateMap<SerpQueueItemDto, SerpQueueItem>();
    }
}
