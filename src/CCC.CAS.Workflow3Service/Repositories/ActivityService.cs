using System;
using System.Threading.Tasks;
using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.Workflow3Service.Services;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow;
using Guflow.Decider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CCC.CAS.Workflow3Service.Repositories
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class ActivityService : IActivityService
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<ActivityService> _logger;
        private readonly Domain _domain;

        public ActivityService(IOptions<AwsWorkflowOptions> config, ILogger<ActivityService> logger, Domain domain)
        {
            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }

        public async Task StartWorkflow(IStartPpo startPpo)
        {
            var workflowId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting workflow with id {workflowId}", workflowId);

            StartWorkflowRequest request = startPpo.ClientCode switch
            {
                "geico" => StartWorkflowRequest.For<PpoWorkflowSignaled>(workflowId),
                _ => StartWorkflowRequest.For<PpoWorkflow>(workflowId),
            };
            request.TaskListName = _config.DefaultTaskList;
            request.Input = startPpo;
            await _domain.StartWorkflowAsync(request).ConfigureAwait(false);
        }
    }

#pragma warning restore CA1812
}
