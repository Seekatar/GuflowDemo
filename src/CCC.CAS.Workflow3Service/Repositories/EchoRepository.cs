using System.Threading.Tasks;
using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Models;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.Utilities;
using Microsoft.Extensions.Configuration;

namespace CCC.CAS.Workflow3Service.Repositories
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    /// <summary>
    /// repository for the gateway-type service that sends a command to another service
    /// </summary>
    internal class EchoRepository : IEchoRepository
    {
        public EchoRepository()
        {
        }
        public Task<EchoResponse?> GetEchoAsync(CallerIdentity identity, string name)
        {
            return Task.FromResult<EchoResponse?>(new EchoResponse { Parm = new EchoResponseParm { Name = name, Message = $"Hi there, {name}! SqlProxy is disabled so it wasn't called", Client = "???" } } );
        }
    }
#pragma warning restore CA1812
}
