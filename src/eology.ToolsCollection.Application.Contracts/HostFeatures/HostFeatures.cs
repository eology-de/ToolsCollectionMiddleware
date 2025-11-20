using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace eology.ToolsCollection.HostFeatures
{
    public class HostFeatures : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            // API Configurations
            var seRankingGroup = context.AddGroup("SeRankingApi");
            seRankingGroup.AddFeature(
               "SeRankingApi.ApiKey",
               defaultValue: "",
               displayName: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:ApiKey"),
               valueType: new FreeTextStringValueType(),
               isVisibleToClients: false
            );

            seRankingGroup.AddFeature(
               "SeRankingApi.SerpRequestExpiration",
               defaultValue: "0",
               description: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:SerpRequestExpiration:Description"),
               displayName: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:SerpRequestExpiration"),
               valueType: new FreeTextStringValueType(new NumericValueValidator(1, 24)),
               isVisibleToClients: false
            );

            seRankingGroup.AddFeature(
               "SeRankingApi.SerpExpiration",
               defaultValue: "7",
               description: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:SerpExpiration:Description"),
               displayName: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:SerpExpiration"),
               valueType: new FreeTextStringValueType(new NumericValueValidator(1, 10)),
               isVisibleToClients: false
            );

            seRankingGroup.AddFeature(
               "SeRankingApi.MaxKeywordCount",
               defaultValue: "1000",
               //description: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:MaxKeywordCount:Description"),
               displayName: LocalizableString.Create<HostFeatures>("HostFeatures:SeRankingApi:MaxKeywordCount"),
               valueType: new FreeTextStringValueType(new NumericValueValidator(1, 1000)),
               isVisibleToClients: true
            );
        }
    }
}

