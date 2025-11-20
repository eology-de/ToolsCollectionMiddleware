using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using eology.SharedLibs.ExternalApis.SERanking;
using eology.ToolsCollection.Features;
using eology.ToolsCollection.Features.Serps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace eology.ToolsCollection
{
    public class SeRankingSerpsBackgroundWorker : HangfireBackgroundWorkerBase
    {
        private readonly IRepository<SerpItem, Guid> _serpItemRepository;
        private readonly IRepository<SerpQueueItem, Guid> _serpQueueItemRepository;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        private readonly IFeatureChecker _featureChecker;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public SeRankingSerpsBackgroundWorker(
            IFeatureChecker featureChecker,
            IRepository<SerpItem, Guid> serpItemRepository,
            IRepository<SerpQueueItem, Guid> serpQueueItemRepository,
            IRepository<IdentityUser, Guid> identityUserRepository,
            IEmailSender emailSender,
            IConfiguration configuration
            )
        {
            _emailSender = emailSender;
            _featureChecker = featureChecker;
            _configuration = configuration;

            _serpQueueItemRepository = serpQueueItemRepository;
            _serpItemRepository = serpItemRepository;

            _identityUserRepository = identityUserRepository;

            RecurringJobId = nameof(SeRankingSerpsBackgroundWorker);

#if DEBUG
            CronExpression = "*/1 * * * *";
#else
            CronExpression = "*/2 * * * *";
#endif
        }

        public async override Task<Task> DoWorkAsync(CancellationToken cancellationToken = default)
        {
            using var uow = LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>().Begin();
            {
                Logger.LogInformation("SeRankingSerpsBackgroundWorker: Start");

                var serApiKey = await _featureChecker.GetOrNullAsync("SeRankingApi.ApiKey");
                string serpQueueItemExpiration = await _featureChecker.GetOrNullAsync("SeRankingApi.SerpRequestExpiration") ?? "0";
                int expirationTimespan = int.Parse(serpQueueItemExpiration);

                if (!string.IsNullOrEmpty(serApiKey) && expirationTimespan > 0)
                {
                    var serpQueueQueriable = await _serpQueueItemRepository.GetQueryableAsync();
                    var userQueryable = await _identityUserRepository.GetQueryableAsync();
                    var serpQueryable = await _serpItemRepository.GetQueryableAsync();

                    var sorting = $"{nameof(SerpQueueItem.CreationTime)} ASC";

                    var baseQuery = serpQueueQueriable
                        .Where(serpQueue => serpQueue.Status == SerpQueueItemStateEnum.Running && serpQueue.TaskId > 0)
                        .OrderBy(sorting)
                        .Take(200);

                    var queryResult = baseQuery.ToList();

                    HashSet<Guid> editedSerps = new();

                    int count = 1;
                    List<SerpQueueItem> queueItemUpdates = new();

                    DateTime expireDate = DateTime.Now.AddHours(expirationTimespan * (-1));

                    /* The results are stored for 24 hours in SeRanking, then they are deleted */
                    foreach (var queueItem in queryResult)
                    {
                        SerpQueueItem queueItemUpdated = new();

                        if (queueItem.CreationTime < expireDate)
                        {
                            queueItem.Status = SerpQueueItemStateEnum.Error;
                            queueItemUpdates.Add(queueItem);
                            editedSerps.Add(queueItem.ParentId);
                        }
                        else
                        {
                            /* API limit 5 requests per second */
                            if (count >= 5)
                            {
                                Thread.Sleep(1000);
                                count = 1;
                            }

                            SER_SERPResult? serSerpQueueResult = await SERankingSerpService.GetTaskItemSerpResult(queueItem.TaskId, serApiKey);

                            if (serSerpQueueResult != null)
                            {
                                queueItem.QueryResultSerialized = JsonConvert.SerializeObject(serSerpQueueResult.results);
                                queueItem.Status = SerpQueueItemStateEnum.Finished;
                                queueItemUpdates.Add(queueItem);
                                editedSerps.Add(queueItem.ParentId);
                            }

                            count++;
                        }
                    }

                    if (queueItemUpdates.Count > 0)
                    {
                        await _serpQueueItemRepository.UpdateManyAsync(queueItemUpdates, autoSave: true, cancellationToken: cancellationToken);

                        if (editedSerps.Count > 0)
                        {
                            var queryable = await _serpItemRepository.GetQueryableAsync();

                            var notificationSerps = queryable
                                .Where(x => editedSerps.Contains(x.Id))
                                .Join(
                                   userQueryable,
                                   serp => serp.CreatorId,
                                   user => user.Id,
                                   (serp, user) => new
                                   {
                                       SerpItem = serp,
                                       UserMail = user.Email
                                   })
                                .Select(x => new
                                {
                                    x.SerpItem,
                                    x.UserMail,
                                    RunningQueueItemCount = x.SerpItem.QueueItems.Where(q => q.Status == SerpQueueItemStateEnum.Running).Count()
                                })
                                .Where(x => x.RunningQueueItemCount == 0)
                                .ToList();

                            foreach (var serp in notificationSerps)
                            {
                                await SendMail(serp.SerpItem, serp.UserMail);
                            }
                        }
                    }
                }
                else
                    Logger.LogError("SeRankingSerpsBackgroundWorker: No API Key found!");

                Logger.LogInformation("SeRankingSerpsBackgroundWorker: End");

                return Task.CompletedTask;
            }
        }

        private async Task<Task> SendMail(SerpItem serp, string email)
        {
            DateTime today = DateTime.Today;

            string host = _configuration.GetSection("App:ClientUrl").Value ?? "";
            string serplistUrl = host + "/serps?";
            string link = serplistUrl + "id=" + serp.Id;

            string content = "<b>" + today.ToString("dd.MM.yyyy") + "</b><br>";
            content += "<h1>SERanking request finished</h1>";
            content += "<p><a href='" + link + "'>" + serp.RequestName + "</a></p>";

            await _emailSender.SendAsync(
                email, // target email address
                "SERanking request " + serp.RequestName + " finished " + today.ToString("dd.MM.yyyy"), // subject
                content, // email body
                true // html
            );
            return Task.CompletedTask;
        }
    }
}