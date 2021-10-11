using CCC.CAS.Workflow3Messages.Messages;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Interfaces
{
    public interface IActivityService
    {
        Task StartPpoWorkflow(IStartPpo scenario);
        Task StartWorkflow(IStartWorkflow scenario);
    }

}
