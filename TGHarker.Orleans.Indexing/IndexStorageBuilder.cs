using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TGHarker.Orleans.Indexing
{
    public class IndexStorageBuilder : IIndexStorageBuilder
    {
        public string Name { get; set; }
        public Action<IndexStorageOptions>? ConfigureOptions { get; set; } = (x) => { };
        public IndexStorageOptions Options { get; private set; }
        public IServiceCollection Services { get; set; }
    }
}
