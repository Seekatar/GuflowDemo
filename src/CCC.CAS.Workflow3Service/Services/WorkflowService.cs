using System;
using System.Threading.Tasks;
using CCC.CAS.AwsWorkflow;
using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow;
using Guflow.Decider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CCC.CAS.Workflow3Service.Services
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class WorkflowService : IWorkflowService
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<WorkflowService> _logger;
        private readonly Domain _domain;

        public WorkflowService(IOptions<AwsWorkflowOptions> config, ILogger<WorkflowService> logger, Domain domain)
        {
            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }

        public async Task StartWorkflow(IStartWorkflow scenario)
        {
            var workflowId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting workflow with id {workflowId}", workflowId);

            StartWorkflowRequest request = StartWorkflowRequest.For<MainWorkflow>(workflowId);

            request.TaskListName = _config.DefaultTaskList;
            request.Input = scenario; // can't be null
            await _domain.StartWorkflowAsync(request).ConfigureAwait(false);
        }

        public async Task StartPpoWorkflow(IStartPpo startPpo)
        {
            var workflowId = Guid.NewGuid().ToString();
            _logger.LogInformation("Starting workflow with id {workflowId}", workflowId);

            StartWorkflowRequest request = startPpo.ClientCode switch
            {
                "geico" => StartWorkflowRequest.For<PpoWorkflowSignaled>(workflowId),
                "usaa" => StartWorkflowRequest.For<PpoWorkflowSignaledDynamic>(workflowId),
                _ => StartWorkflowRequest.For<PpoWorkflow>(workflowId),
            };
            request.TaskListName = _config.DefaultTaskList;
            request.Input = startPpo;
            await _domain.StartWorkflowAsync(request).ConfigureAwait(false);
        }
    }

#pragma warning restore CA1812
}
