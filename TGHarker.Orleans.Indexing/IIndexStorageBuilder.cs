using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TGHarker.Orleans.Indexing
{
    public interface IIndexStorageBuilder
    {
        string Name { get; set; }
        IndexStorageOptions Options { get; }
        IServiceCollection Services { get; set; }
    }
}
