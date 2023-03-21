using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public sealed record AzureSearchOptions
    {
        public required Uri Uri { get; init; }
        public required string ApiKey { get; init; }
    }
}
