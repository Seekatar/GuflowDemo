using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Activities
{
    [ActivityDescription("1.3",
        DefaultTaskListName = "defaultTaskList",
        DefaultHeartbeatTimeoutInSeconds = 10000,
        DefaultScheduleToCloseTimeoutInSeconds = 1000,
        DefaultScheduleToStartTimeoutInSeconds = 1000,
        DefaultStartToCloseTimeoutInSeconds = 1000
        )]
    public class PpoEnd : CasActvity<PpoEnd>
    {
        public PpoEnd(IOptions<AwsWorkflowOptions> config, ILogger<PpoEnd> logger, Domain domain) : base(config, logger, domain)
        {
        }

        [ActivityMethod]
        public async Task<ActivityResponse> Execute(ActivityArgs _)
        {
            Logger.LogInformation($">>>>>>>>>> {GetType().Name} processing...");

            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            return Complete(new { Started = true });
        }
    }
}