using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Models;
using System;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Interfaces
{
    public interface IWorkflow3Repository
    {
        Task<Workflow3?> SaveWorkflow3Async(CallerIdentity identity, Workflow3 item, Guid? correlationId);
        Task<Workflow3?> GetWorkflow3Async(CallerIdentity identity, string workflow3Id);
        Task DeleteWorkflow3Async(CallerIdentity identity, string workflow3Id);
    }

}
