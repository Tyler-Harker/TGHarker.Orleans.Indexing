using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing
{
    public class IndexStorageProvider : IGrainStorage
    {
        private readonly string _name;
        private readonly Dictionary<string, IGrainStorage> _storageProviders = new Dictionary<string, IGrainStorage>();
        private readonly IIndexStorageProvider _indexStorageProvider;
        private readonly IndexStorageOptions _options;
        private Dictionary<Type, Type?> GrainStateTypeToGeneratedStateType { get; set; } = new();
        private Dictionary<Type, IBaseIndexState> GeneratedStateTypeToInstance { get; set; } = new();
        private Dictionary<Type, string> IndexNames { get; set; } = new();
        public IndexStorageProvider(IServiceProvider services, string name, IndexStorageOptions options)
        {
            _options = options;
            _indexStorageProvider =
                services.GetServices<IKeyedServiceCollection<string, IIndexStorageProvider>>()
                    .SelectMany(c => c.GetServices(services)).FirstOrDefault(x => x.Key == name).GetService(services) ??
                throw new Exception($"Unable to find an index storage provider configured with name: {name}");
            

            _name = name;
            IEnumerable<IKeyedServiceCollection<string, IGrainStorage>> namedIControllableCollections = services.GetServices<IKeyedServiceCollection<string, IGrainStorage>>();
            foreach (IKeyedService<string, IGrainStorage> keyedService in namedIControllableCollections.SelectMany(c => c.GetServices(services)))
            {
                if (_name == keyedService.Key)
                {
                    continue;
                }
                IGrainStorage controllable = keyedService.GetService(services);
                if (controllable != null)
                {
                    _storageProviders.Add(keyedService.Key, controllable);
                }
            }

            foreach (var assembly in options.GrainStateAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types) //this for loop and loop below it have to be separate. Have to make sure index names are created.
                {
                    if (type.IsDefined(typeof(IndexableStateAttribute), false))
                    {
                        var attribute = (IndexableStateAttribute)type.GetCustomAttributes(typeof(IndexableStateAttribute)).First();
                        var indexName = attribute.Name ?? type.Name;

                        IndexNames.Add(type, indexName.ToLower());
                    }
                }
                foreach (var type in types)
                {
                    
                    if (type.IsDefined(typeof(GeneratedIndexClassAttribute), false))
                    {
                        var originalStateType = type.BaseType.GetGenericArguments()[0];
                        GrainStateTypeToGeneratedStateType.Add(originalStateType, type);
                        GeneratedStateTypeToInstance.Add(type, (IBaseIndexState)Activator.CreateInstance(type));
                        _indexStorageProvider.CreateIndexAsync(IndexNames[originalStateType], type).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    
                }
            }
        }

        protected IGrainStorage GetStorageProviderByName(string name)
        {
            if (_storageProviders.ContainsKey(name) is false)
            {
                throw new Exception($"No Grain Storage Provider configured with name: {name}");
            }

            return _storageProviders[name];
        }

        public Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            return GetStorageProviderByName(_options.PrimaryStorageProviderName).ReadStateAsync(stateName, grainId, grainState);
        }

        public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            await GetStorageProviderByName(_options.PrimaryStorageProviderName).WriteStateAsync<T>(stateName, grainId, grainState);

            if (GrainStateTypeToGeneratedStateType.ContainsKey(grainState.State.GetType()))
            {
                var grainStateType = grainState.State.GetType();
                var generatedGrainStateType = GrainStateTypeToGeneratedStateType[grainStateType];
                var generatedObj = GeneratedStateTypeToInstance[generatedGrainStateType];
                generatedObj.Map(grainState.State);

                if (_options.AwaitIndexSave)
                {
                    await _indexStorageProvider.UploadAsync(IndexNames[grainStateType], generatedObj);
                }
                else
                {
                    _indexStorageProvider.UploadAsync(IndexNames[grainStateType], generatedObj);
                }
            }
            
        }

        public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            return GetStorageProviderByName(_options.PrimaryStorageProviderName).ClearStateAsync<T>(stateName, grainId, grainState);
        }
    }
}
