using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.JsonConverters;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Events;
using VirtoCommerce.ShippingModule.Core.Extensions;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Search.Indexed;
using VirtoCommerce.ShippingModule.Core.Security;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data;
using VirtoCommerce.ShippingModule.Data.ExportImport;
using VirtoCommerce.ShippingModule.Data.Handlers;
using VirtoCommerce.ShippingModule.Data.MySql;
using VirtoCommerce.ShippingModule.Data.PostgreSql;
using VirtoCommerce.ShippingModule.Data.Repositories;
using VirtoCommerce.ShippingModule.Data.Search.Indexed;
using VirtoCommerce.ShippingModule.Data.Services;
using VirtoCommerce.ShippingModule.Data.SqlServer;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        private IApplicationBuilder _appBuilder;
        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<ShippingDbContext>((provider, options) =>
            {
                var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });


            serviceCollection.AddTransient<IShippingRepository, ShippingRepository>();
            serviceCollection.AddTransient<Func<IShippingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IShippingRepository>());

            serviceCollection.AddTransient<IPickupLocationService, PickupLocationService>();
            serviceCollection.AddTransient<IPickupLocationSearchService, PickupLocationSearchService>();

            serviceCollection.AddTransient<IShippingMethodsService, ShippingMethodsService>();
            serviceCollection.AddTransient<IShippingMethodsRegistrar, ShippingMethodsService>();
            serviceCollection.AddTransient<IShippingMethodsSearchService, ShippingMethodsSearchService>();

            serviceCollection.AddTransient<ShippingExportImport>();

            serviceCollection.AddTransient<IndexPickupLocationChangedEventHandler>();

            serviceCollection.AddTransient<IPickupLocationIndexedSearchService, PickupLocationIndexedSearchService>();
            serviceCollection.AddTransient<PickupLocationSearchRequestBuilder>();

            serviceCollection.AddSingleton<PickupLocationChangesProvider>();
            serviceCollection.AddSingleton<PickupLocationDocumentBuilder>();

            serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
            {
                DocumentType = ModuleConstants.PickupLocationIndexDocumentType,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = provider.GetService<PickupLocationChangesProvider>(),
                    DocumentBuilder = provider.GetService<PickupLocationDocumentBuilder>(),
                },
            });
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settings = ModuleConstants.Settings.AllSettings;

            if (Configuration.IsPickupLocationFullTextSearchEnabled())
            {
                settings = settings.Union(ModuleConstants.Settings.General.IndexationSettings);
            }

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(settings, ModuleInfo.Id);
            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.FixedRateShippingMethod.AllSettings, typeof(FixedRateShippingMethod).Name);
            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.StoreSettings, nameof(Store));

            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "Shipping", ModuleConstants.Security.Permissions.AllPermissions);

            // Register permission scopes
            AbstractTypeFactory<PermissionScope>.RegisterType<SelectedStoreScope>();

            permissionsRegistrar.WithAvailabeScopesForPermissions(
                [
                    ModuleConstants.Security.Permissions.Read,
                    ModuleConstants.Security.Permissions.Update,
                    ModuleConstants.Security.Permissions.Delete,
                    ModuleConstants.Security.Permissions.Create,
                ],
                new SelectedStoreScope());

            appBuilder.RegisterEventHandler<PickupLocationChangedEvent, IndexPickupLocationChangedEventHandler>();

            var shippingMethodsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IShippingMethodsRegistrar>();
            shippingMethodsRegistrar.RegisterShippingMethod<FixedRateShippingMethod>();
            shippingMethodsRegistrar.RegisterShippingMethod<BuyOnlinePickupInStoreShippingMethod>();

            PolymorphJsonConverter.RegisterTypeForDiscriminator(typeof(ShippingMethod), nameof(ShippingMethod.TypeName));

            var searchRequestBuilderRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISearchRequestBuilderRegistrar>();
            searchRequestBuilderRegistrar.Register(ModuleConstants.PickupLocationIndexDocumentType, appBuilder.ApplicationServices.GetService<PickupLocationSearchRequestBuilder>);

            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");

            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ShippingDbContext>();
            if (databaseProvider == "SqlServer")
            {
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
            }
            dbContext.Database.Migrate();
        }

        public void Uninstall()
        {
            // This method intentionally left empty
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
          ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ShippingExportImport>().DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ShippingExportImport>().DoImportAsync(inputStream, progressCallback, cancellationToken);
        }
    }
}
