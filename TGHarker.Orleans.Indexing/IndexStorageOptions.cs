using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TGHarker.Orleans.Indexing
{
    public sealed class IndexStorageOptions
    {
        public string PrimaryStorageProviderName { get; set; }
        public bool AwaitIndexSave { get; set; } = false;
        public IEnumerable<Assembly> GrainStateAssemblies { get; set; } = new List<Assembly>();
    }
}
