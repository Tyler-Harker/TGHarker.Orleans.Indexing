using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGHarker.Orleans.Indexing.SiloSample.GrainInterfaces;

namespace TGHarker.Orleans.Indexing.SiloSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public UsersController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        [HttpGet("{id:int}")]
        public Task<string> GetFullName(int id)
        {
            return _clusterClient.GetGrain<IUserGrain>(id).GetFullName();
        }

        [HttpPost("{id:int}")]
        public Task SetFullName(int id, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            return _clusterClient.GetGrain<IUserGrain>(id).SetName(firstName, lastName);
        }
    }
}
