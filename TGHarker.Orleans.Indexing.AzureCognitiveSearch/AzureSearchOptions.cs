using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TGHarker.Orleans.Indexing.AzureCognitiveSearch
{
    public sealed record AzureSearchOptions
    {
        [JsonIgnore]
        public Uri Uri { get; set; }
        public string ApiKey { get; set; }
    }
}
