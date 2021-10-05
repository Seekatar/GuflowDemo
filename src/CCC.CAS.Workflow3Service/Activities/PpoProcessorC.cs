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
    public class PpoProcessorC : Activity
    {
        private readonly AwsWorkflowConfiguration _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;
        private readonly Domain _domain;

        public PpoProcessorC(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowDeciderService> logger, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }

        [ActivityMethod]
        public ActivityResponse Execute(ActivityArgs args)
        {
            Task.Run(() => {
                string workflowId = args.WorkflowId;
                string runId = args.WorkflowRunId;
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
