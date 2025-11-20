using System;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;
using Volo.Abp.Uow;
using eology.ToolsCollection.Features;
using System.Linq;

namespace eology.ToolsCollection
{
    public class DbCleanerBackgroundWorker : HangfireBackgroundWorkerBase
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<SerpItem, Guid> _serpItemRepository;

        public DbCleanerBackgroundWorker(
            IFeatureChecker featureChecker,
            IRepository<SerpItem, Guid> serpItemRepository
            )
        {
            _serpItemRepository = serpItemRepository;
            _featureChecker = featureChecker;

            RecurringJobId = nameof(DbCleanerBackgroundWorker);

#if DEBUG
            CronExpression = "*/10 * * * *";
#else
            CronExpression = Cron.Daily();
#endif
        }

        public async override Task<Task> DoWorkAsync(CancellationToken cancellationToken = default)
        {
            using var uow = LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>().Begin();
            var serpItemRepository = _serpItemRepository;

            /*
             * Delete expired serpItems
             */
            Logger.LogInformation("DbCleanerBackgroundWorker: Start deleting expired serpItems");

            string serpItemExpiration = await _featureChecker.GetOrNullAsync("SeRankingApi.SerpExpiration") ?? "0";
            int expirationTimespan = int.Parse(serpItemExpiration);
            if (expirationTimespan >= 1)
            {
                var expiredQuery = await serpItemRepository.GetQueryableAsync();

                DateTime expiryDateSerpItems = DateTime.Today.AddDays(expirationTimespan * (-1));
                expiredQuery = expiredQuery.Where(x => x.CreationTime < expiryDateSerpItems);

                var expiredSerpItems = expiredQuery.ToList();

                await serpItemRepository.DeleteManyAsync(expiredSerpItems, true, cancellationToken);

                Logger.LogInformation("DbCleanerBackgroundWorker: {Count} serpItems successfully deleted!", expiredSerpItems.Count);
            }
            else
            {
                Logger.LogError("DbCleanerBackgroundWorker: Wrong deleteTimeSpan \"{Field}\"", serpItemExpiration);
            }

            return Task.CompletedTask;
        }
    }

}

