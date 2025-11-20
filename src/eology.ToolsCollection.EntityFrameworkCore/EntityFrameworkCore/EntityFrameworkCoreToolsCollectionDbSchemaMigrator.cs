using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using eology.ToolsCollection.Data;
using Volo.Abp.DependencyInjection;

namespace eology.ToolsCollection.EntityFrameworkCore;

public class EntityFrameworkCoreToolsCollectionDbSchemaMigrator
    : IToolsCollectionDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreToolsCollectionDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the ToolsCollectionDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ToolsCollectionDbContext>()
            .Database
            .MigrateAsync();
    }
}
