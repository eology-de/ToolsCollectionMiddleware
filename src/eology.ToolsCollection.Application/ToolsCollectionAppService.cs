using System;
using System.Collections.Generic;
using System.Text;
using eology.ToolsCollection.Localization;
using Volo.Abp.Application.Services;

namespace eology.ToolsCollection;

/* Inherit your application services from this class.
 */
public abstract class ToolsCollectionAppService : ApplicationService
{
    protected ToolsCollectionAppService()
    {
        LocalizationResource = typeof(ToolsCollectionResource);
    }
}
