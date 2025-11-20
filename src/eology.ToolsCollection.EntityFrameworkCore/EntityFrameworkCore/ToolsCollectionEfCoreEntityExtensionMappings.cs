using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;
using eology.ToolsCollection.Domain.Shared;

namespace eology.ToolsCollection.EntityFrameworkCore;

public static class ToolsCollectionEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        ToolsCollectionGlobalFeatureConfigurator.Configure();
        ToolsCollectionModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE ToolsCollectionModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );

             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */
                 
                  // TotpSecret auf eine dedizierte Spalte in der IdentityUser-Tabelle abbilden
                ObjectExtensionManager.Instance.MapEfCoreProperty<IdentityUser, string>(
                    UserConsts.TotpSecretPropertyName,
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(256); // Geeignete Länge für Base32-kodiertes Geheimnis
                        propertyBuilder.IsRequired(false); // Kann null sein, wenn 2FA nicht aktiviert ist
                        propertyBuilder.HasColumnName("TotpSecret"); // Expliziter Spaltenname
                    }
                );

                // RecoveryCodes auf eine dedizierte Spalte abbilden (z.B. als JSON-String)
                ObjectExtensionManager.Instance.MapEfCoreProperty<IdentityUser, string>(
                    UserConsts.RecoveryCodesPropertyName,
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(1024); // Ausreichend für mehrere Codes
                        propertyBuilder.IsRequired(false);
                        propertyBuilder.HasColumnName("RecoveryCodes");
                    }
                );
        });
    }
}
