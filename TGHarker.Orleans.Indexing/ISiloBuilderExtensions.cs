using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Providers;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans.Runtime;

namespace TGHarker.Orleans.Indexing
{
    public static class ISiloBuilderExtensions
    {
        public static ISiloBuilder AddIndexing(this ISiloBuilder builder, string name, Action<IndexStorageBuilder> configureBuilder)
        {
            return builder.ConfigureServices(services =>
                services.AddIndexing(name, configureBuilder));
        }
        public static ISiloBuilder AddIndexing(this ISiloBuilder builder, Action<IndexStorageBuilder> configureBuilder)
        {
            return builder.ConfigureServices(services =>
                services.AddIndexing(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureBuilder));
        }

        internal static IServiceCollection AddIndexing(this IServiceCollection services, string name,
            Action<IndexStorageBuilder> configureBuilder = null)
        {
            var indexStorageBuilder = new IndexStorageBuilder()
            {
                Services = services,
                Name = name
            };
            configureBuilder.Invoke(indexStorageBuilder);

            services.AddOptions<IndexStorageOptions>(name).Configure(indexStorageBuilder.ConfigureOptions);

            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            services.AddSingletonNamedService<IGrainStorage>(name, IndexStorageFactory.Create);
            return services;
            //.AddSingletonNamedService<ILifecycleParticipant<ISilo>>()
        }
    }
}
