using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Models;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Interfaces
{
    public interface IEchoRepository
    {
        Task<EchoResponse?> GetEchoAsync(CallerIdentity identity, string name);
    }

}
