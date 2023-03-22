using System.Runtime.InteropServices;
using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.SiloSample.GrainStates
{
    [IndexableState]
    public sealed class UserGrainState
    {
        [IndexableProperty(IsId = true)] 
        public string Id { get; set; } = "1";
        [IndexableProperty]
        public string FirstName { get; set; }
        [IndexableProperty]
        public string LastName { get; set; }
    }
}
