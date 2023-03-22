using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Configuration.Overrides;
using Orleans.Runtime;
using Orleans.Storage;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public static class IIndexStorageBuilderExtensions
    {
        public static IIndexStorageBuilder UseAzureCognitiveSearch(this IIndexStorageBuilder builder,
            Action<AzureSearchOptions> configureOptions)
        {
            builder.Services.AddOptions<AzureSearchOptions>(builder.Name).Configure(configureOptions);
            builder.Services.AddSingletonNamedService(builder.Name, Create);
            return builder;
        }

        public static IIndexStorageProvider Create(IServiceProvider services, string name)
        {
            var optionsSnapshot = services.GetRequiredService<IOptionsMonitor<AzureSearchOptions>>();
            var clusterOptions = services.GetProviderClusterOptions(name);
            return ActivatorUtilities.CreateInstance<AzureCognitiveSearchStorageProvider>(services, optionsSnapshot.Get(name));
        }
    }
}
