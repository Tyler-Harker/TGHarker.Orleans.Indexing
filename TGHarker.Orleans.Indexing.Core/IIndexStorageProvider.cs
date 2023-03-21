using System;
using System.Collections.Generic;
using System.Text;

namespace TGHarker.Orleans.Indexing.Core
{
    public interface IIndexStorageProvider
    {
        Task CreateIndexAsync<T>();
        Task UploadAsync<T>(T item);
    }
}
