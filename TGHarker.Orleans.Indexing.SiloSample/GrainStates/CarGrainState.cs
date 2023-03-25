using TGHarker.Orleans.Indexing.Core;

namespace TGHarker.Orleans.Indexing.SiloSample.GrainStates
{
    [IndexableState(Name="Cars")]
    public class CarGrainState
    {
        [IndexableProperty(IsId = true)]
        public string Id { get; set; }
    }
}
