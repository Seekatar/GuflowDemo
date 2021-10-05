using Amazon;
using Amazon.SimpleWorkflow;
using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Activities
{
    [ActivityDescription("1.2",
        DefaultTaskListName ="defaultTaskList", 
        DefaultHeartbeatTimeoutInSeconds = 10000, 
        DefaultScheduleToCloseTimeoutInSeconds = 1000, 
        DefaultScheduleToStartTimeoutInSeconds = 1000, 
        DefaultStartToCloseTimeoutInSeconds = 1000
        )]
    public class PpoProcessorA : Activity
    {
        private readonly AwsWorkflowConfiguration _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;

        public PpoProcessorA(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowDeciderService> logger)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
        }

        [ActivityMethod]
        public ActivityResponse Execute(string _)
        {
            Task.Run(() => {
                string workflowId = "";
                string runId = "";
                Task.Delay(TimeSpan.FromSeconds(2));
                using var swfClient = new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region));
                var domain = new Domain(_config.Domain, swfClient);
                domain.SignalWorkflowAsync(new SignalWorkflowRequest(workflowId, "TestSignal") { 
                    SignalInput = new {Test =1},
                    WorkflowRunId = runId
                }).ConfigureAwait(false);
                
            });
            return Complete(new { Started = true });
        }

    }
}
