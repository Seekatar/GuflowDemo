using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Models;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Interfaces
{
    public interface IActivityService
    {
        Task StartWorkflow(int scenario);
    }

}
