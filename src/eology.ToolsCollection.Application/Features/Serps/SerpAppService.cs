using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using eology.SharedLibs.ExternalApis.SERanking;
using eology.ToolsCollection.Localization;
using eology.ToolsCollection.Permissions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;

namespace eology.ToolsCollection.Features.Serps
{
    [Authorize(ToolsCollectionPermissions.Serps.Default)]
    public class SerpAppService :
    CrudAppService<
        SerpItem, // Database Entity
        SerpItemDto, // Data/Payload Transfer Object
        Guid, //Primary key of the entity
        SerpItemPagedSortedFilteredRequestDto, //Used for paging/sorting
        CreateUpdateSerpItemDto>,//Used to create/update an entity
    ISerpAppService //implement the IBookAppService
    {
        private readonly IRepository<SerpQueueItem, Guid> _serpRequestQueueItemRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IStringLocalizer<ToolsCollectionResource> _localizer;

        public SerpAppService(
            IRepository<SerpItem, Guid> repository,
            IRepository<SerpQueueItem, Guid> serpRequestQueueItemRepository,
            IFeatureChecker featureChecker,
            IStringLocalizer<ToolsCollectionResource> localizer
            )
             : base(repository)
        {
            _serpRequestQueueItemRepository = serpRequestQueueItemRepository;

            _featureChecker = featureChecker;
            _localizer = localizer;

            GetPolicyName = ToolsCollectionPermissions.Serps.Default;
            GetListPolicyName = ToolsCollectionPermissions.Serps.Default;
            CreatePolicyName = ToolsCollectionPermissions.Serps.Create;
            UpdatePolicyName = ToolsCollectionPermissions.Serps.Edit;
            DeletePolicyName = ToolsCollectionPermissions.Serps.Delete;
        }

        public override async Task<SerpItemDto> CreateAsync(CreateUpdateSerpItemDto input)
        {
            SerpItemDto insertedItemAsDto = new();

            var serApiKey = await _featureChecker.GetOrNullAsync("SeRankingApi.ApiKey");

            if (!string.IsNullOrEmpty(serApiKey))
            {               
                var keywords = input.Keywords?.Split(',').ToList();
                var keywordsSanitized = new List<string>();
                keywords?.ForEach(item =>
                {
                    var sanitized = StringHelpers.TrimWhitespace(item);
                    keywordsSanitized.Add(sanitized);
                });

                //Todo: Ausbau Statusrückmeldung Abfrage SERanking, Fehler etc.
                var serankingSerpQueueRequestResult = await SERankingSerpService.RequestSerpsFromSERanking(keywordsSanitized, serApiKey);

                if (serankingSerpQueueRequestResult != null && serankingSerpQueueRequestResult.Count == keywords?.Count)
                {
                    var serpRequestItem = ObjectMapper.Map<CreateUpdateSerpItemDto, SerpItem>(input);
                    //Schreibe RequestItem
                    var insertedItem = await Repository.InsertAsync(serpRequestItem, true); // Speichert das übergeordnete Element und seine Kindelemente [5, 13]

                    var queueItems = new List<SerpQueueItem>();

                    serankingSerpQueueRequestResult?.ForEach(item =>
                        {
                            SerpQueueItem serpQueueItem = new()
                            {
                                ParentId = insertedItem.Id,
                                Query = item.Query,
                                TaskId = item.TaskId,
                                QueryResultSerialized = "",
                                Status = SerpQueueItemStateEnum.Running
                            };

                            queueItems.Add(serpQueueItem);
                        });
                    await _serpRequestQueueItemRepository.InsertManyAsync(queueItems, true);
                    insertedItemAsDto = ObjectMapper.Map<SerpItem, SerpItemDto>(insertedItem);
                }
            }
            else
                throw new Volo.Abp.UserFriendlyException(_localizer["Serp:Error:NoApiKey"]);            

            return insertedItemAsDto;
        }

        public override async Task<SerpItemDto> UpdateAsync(Guid id, CreateUpdateSerpItemDto input)
        {
            var serpItemToUpdate = await Repository.GetAsync(id);

            // Nur die gewünschten Eigenschaften aktualisieren
            serpItemToUpdate.RequestName = input.RequestName;
            serpItemToUpdate.ResultCount = input.ResultCount;

            await Repository.UpdateAsync(serpItemToUpdate);

            return ObjectMapper.Map<SerpItem, SerpItemDto>(serpItemToUpdate);
        }

        public override async Task<PagedResultDto<SerpItemDto>>
            GetListAsync(SerpItemPagedSortedFilteredRequestDto input)
        {
            var baseQuery = await Repository.WithDetailsAsync(x => x.QueueItems);

            if (input.CreatorId != null && input.CreatorId != Guid.Empty)
                baseQuery = baseQuery.Where(x => x.CreatorId == input.CreatorId);

            if (input.Id != null && input.Id != Guid.Empty)
                baseQuery = baseQuery.Where(x => x.Id == input.Id);

            if (!string.IsNullOrWhiteSpace(input.RequestName))
                baseQuery = baseQuery.Where(x => x.RequestName.Contains(input.RequestName));

            var countquery = baseQuery;

            var sorting = NormalizeSorting(input.Sorting);

            baseQuery = baseQuery
              .OrderBy(sorting)
              .Skip(input.SkipCount)
              .Take(input.MaxResultCount);

            var serpItems = baseQuery.ToList();

            var serpItemDtos = new List<SerpItemDto>();
            serpItems.ForEach(serpItem =>
            {
                var serpItemDto = ObjectMapper.Map<SerpItem, SerpItemDto>(serpItem);
                var total = serpItem.QueueItems.Count;
                var part = serpItem.QueueItems.ToList().Where(sItem => sItem.Status != SerpQueueItemStateEnum.Running).ToList().Count;
               
                serpItemDto.Total = total;
                serpItemDto.Part = part;

                serpItemDtos.Add(serpItemDto);
            });

            //Get the total count with another query
            var totalCount = await AsyncExecuter.CountAsync(countquery);

            return new PagedResultDto<SerpItemDto>(
                totalCount,
                serpItemDtos
            );
        }

        private static string NormalizeSorting(string? sorting)
        {
            if (sorting.IsNullOrEmpty())
            {
                return $"{nameof(SerpItem.CreationTime)} DESC";
            }

            return $"{sorting}";
        }

        public async Task<SER_AccountBalance> GetBalance() {

            var serApiKey = await _featureChecker.GetOrNullAsync("SeRankingApi.ApiKey");

            SER_AccountBalance balance = new();

            if (!string.IsNullOrEmpty(serApiKey))
                balance = await SERankingSerpService.GetAccountBalance(serApiKey) ?? new();

            return balance;
        }
    }
}