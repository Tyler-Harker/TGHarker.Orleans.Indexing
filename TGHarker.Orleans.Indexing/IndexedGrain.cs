using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public abstract class IndexedGrain<T, TIndex> : Grain<T>, IIndexableGrain
        where T : new()
        where TIndex : BaseIndexState<T>, new()
    {
        private TIndex MappingObject = new TIndex();
        private static readonly HashSet<Type> Indexes = new HashSet<Type>();

        private readonly IIndexStorageProvider _storageProvider;
        public IndexedGrain(IIndexStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public async Task WriteDataAsync()
        {
            if (Indexes.Contains(typeof(T)) is false)
            {
                await CreateIndexAsync();
                Indexes.Add(typeof(T));
            }

            await UploadToIndexAsync();
        }
        public Task UploadToIndexAsync()
        {
            MappingObject.Map(State);
            return _storageProvider.UploadAsync(MappingObject);
        }

        public Task CreateIndexAsync()
        {
            return _storageProvider.CreateIndexAsync<TIndex>();
        }

        protected override Task WriteStateAsync()
        {
            _ = WriteDataAsync();
            return base.WriteStateAsync();
        }
    }
}
