using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public interface IIndexStorageProvider
    {
        Task CreateIndexAsync(string indexName, Type type);
        Task UploadAsync(string indexName, object item);
    }
}
