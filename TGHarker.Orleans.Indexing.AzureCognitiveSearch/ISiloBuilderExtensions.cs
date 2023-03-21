using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public static class ISiloBuilderExtensions
    {
        public static ISiloBuilder AddAzureCognitiveSearchIndexing(this ISiloBuilder builder,
            AzureSearchOptions options)
        {
            builder.Services.AddSingleton(options);
            builder.Services.AddTransient<IIndexStorageProvider, AzureCognitiveSearchStorageProvider>();
            return builder;
        }
    }
}
