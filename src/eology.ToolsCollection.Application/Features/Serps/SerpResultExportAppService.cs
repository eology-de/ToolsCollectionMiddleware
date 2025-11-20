using System;
using System.Linq;
using System.Threading.Tasks;

using eology.ToolsCollection.Features.ExcelExport;
using eology.ToolsCollection.Permissions;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace eology.ToolsCollection.Features.Serps
{
    [Authorize(ToolsCollectionPermissions.Serps.Default)]
    public class SerpResultExportAppService :
        ReadOnlyAppService<
            SerpItem, //The Book entity
            SerpItemDto, //Used to show books
            Guid, //Primary key of the book entity
            SerpItemPagedSortedFilteredRequestDto //Used for paging/sorting
            >,
        ISerpResultExportAppService //implement the IBookAppService
    {
        public SerpResultExportAppService(
              IRepository<SerpItem, Guid> repository
            ) : base(repository)
        { }

        public async Task<byte[]> ExportReport(Guid serpItemId)
        {
            var allSerpItems = await Repository.WithDetailsAsync(x => x.QueueItems);
            var serpItem = allSerpItems.Where(item => item.Id == serpItemId).FirstOrDefault();

            if (serpItem != null)
            {
                string filename = serpItem.RequestName + "_" + DateTime.Now.ToString() + ".xlsx";
                var data = ExcelServiceByEpplus.WriteExcelSerpReport(serpItem, filename);
                return data;
            }
            else
            {
                throw new UserFriendlyException("Der Eintrag mit der Id " + serpItemId + " wurde nicht gefunden.");
            }
        }
    }
}