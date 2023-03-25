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
        private SearchClient _searchClient;
        private readonly FieldBuilder _fieldBuilder = new FieldBuilder();
        private readonly AzureSearchOptions _options;
        public AzureCognitiveSearchStorageProvider(AzureSearchOptions options)
        {
            _options = options;
        }

        public Task CreateIndexAsync(string indexName, Type type)
        {
            var adminCredentials = new AzureKeyCredential(_options.ApiKey);
            var indexClient = new SearchIndexClient(_options.Uri, adminCredentials);
            var index = new SearchIndex(indexName)
            {
                Fields = _fieldBuilder.Build(type)
            };
            var response = indexClient.CreateOrUpdateIndex(index);
            return Task.CompletedTask;
        }

        public Task UploadAsync(string indexName, object o)
        {
            _searchClient = _searchClient ?? new SearchClient(_options.Uri, indexName, new AzureKeyCredential(_options.ApiKey));
            _searchClient.UploadDocuments(new [] { o });
            return Task.CompletedTask;
        }

    }
}
