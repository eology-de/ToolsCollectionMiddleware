using eology.ToolsCollection.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace eology.ToolsCollection.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ToolsCollectionController : AbpControllerBase
{
    protected ToolsCollectionController()
    {
        LocalizationResource = typeof(ToolsCollectionResource);
    }
}
