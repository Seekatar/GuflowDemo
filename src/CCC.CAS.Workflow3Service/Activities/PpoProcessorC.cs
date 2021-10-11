using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CCC.CAS.Workflow3Service.Activities
{
    [ActivityDescription("1.3",
        DefaultTaskListName = "defaultTaskList",
        DefaultHeartbeatTimeoutInSeconds = 10000,
        DefaultScheduleToCloseTimeoutInSeconds = 1000,
        DefaultScheduleToStartTimeoutInSeconds = 1000,
        DefaultStartToCloseTimeoutInSeconds = 1000
        )]
    public class PpoProcessorC : PpoProcessor<PpoProcessorC>
    {
        public PpoProcessorC(IOptions<AwsWorkflowOptions> config, ILogger<PpoProcessorC> logger, Domain domain) : base(config, logger, domain)
        {
        }
        public static Identity Identity => Identity.New(nameof(PpoProcessorC), "1.3");
    }
}
