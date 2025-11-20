using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace eology.ToolsCollection;

[DependsOn(
    typeof(ToolsCollectionDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(ToolsCollectionApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpBackgroundWorkersModule)
    )]
public class ToolsCollectionApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ToolsCollectionApplicationModule>();
        });
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {

        await context.AddBackgroundWorkerAsync<SeRankingSerpsBackgroundWorker>();

#if !DEBUG
        await context.AddBackgroundWorkerAsync<SeRankingSerpsBackgroundWorker>();
        await context.AddBackgroundWorkerAsync<DbCleanerBackgroundWorker>();
#endif
    }
}
