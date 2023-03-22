using Orleans.Runtime;
using TGHarker.Orleans.Indexing.SiloSample.GrainInterfaces;
using TGHarker.Orleans.Indexing.SiloSample.GrainStates;

namespace TGHarker.Orleans.Indexing.SiloSample.Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly IPersistentState<UserGrainState> _state;
        public UserGrain([PersistentState(nameof(UserGrain))] IPersistentState<UserGrainState> state)
        {
            _state = state;
        }
        public Task SetName(string firstName, string lastName)
        {
            _state.State.FirstName = firstName;
            _state.State.LastName = lastName;
            return _state.WriteStateAsync();
        }

        public Task<string?> GetFullName()
        {
            return Task.FromResult<string?>($"{_state.State.FirstName} {_state.State.LastName}");
        }
    }
}
