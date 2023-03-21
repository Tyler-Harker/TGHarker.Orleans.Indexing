namespace TGHarker.Orleans.Indexing.Core
{
    public interface IIndexableGrain
    {
        Task WriteDataAsync();
    }
}
