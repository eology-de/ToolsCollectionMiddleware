using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace eology.ToolsCollection.Features.Serps
{
	public interface ISerpAppService :
    ICrudAppService< //Defines CRUD methods
        SerpItemDto, //Used to show books
        Guid, //Primary key of the book entity
        SerpItemPagedSortedFilteredRequestDto, //Used for paging/sorting
        CreateUpdateSerpItemDto> //Used to create/update a book
    {

    }
}