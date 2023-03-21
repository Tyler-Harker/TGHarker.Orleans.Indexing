using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public sealed class AzureCognitiveSearchStorageProvider : IIndexStorageProvider
    {
        private readonly SearchClient _searchClient;
        private readonly FieldBuilder _fieldBuilder = new FieldBuilder();
        private readonly AzureSearchOptions _options;
        public AzureCognitiveSearchStorageProvider(AzureSearchOptions options)
        {
            _options = options;
        }

        public Task CreateIndexAsync<T>()
        {
            var adminCredentials = new AzureKeyCredential(_options.ApiKey);
            var indexClient = new SearchIndexClient(_options.Uri, adminCredentials);
            var index = new SearchIndex(GetIndexName<T>())
            {
                Fields = _fieldBuilder.Build(typeof(T))
            };
            var response = indexClient.CreateOrUpdateIndex(index);
            return Task.CompletedTask;
        }

        public Task UploadAsync<T>(T item)
        {
            return Task.CompletedTask;
        }

        private string GetIndexName<T>() => typeof(T).Name.ToLower();
    }
}
