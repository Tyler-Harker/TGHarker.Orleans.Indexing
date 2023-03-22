namespace TGHarker.Orleans.Indexing.SiloSample.GrainInterfaces
{
    public interface IUserGrain : IGrainWithIntegerKey
    {
        Task SetName(string firstName, string lastName);
        Task<string?> GetFullName();
    }
}
