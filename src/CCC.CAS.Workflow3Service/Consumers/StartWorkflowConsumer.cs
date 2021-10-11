using CCC.CAS.Workflow3Messages.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.API.Common.Logging;
using CCC.CAS.Workflow3Messages.Models;

namespace CCC.CAS.Workflow3Service.Consumers
{
    public class StartWorkflowConsumer : IConsumer<IStartWorkflow>
    {
        private readonly ILogger<StartWorkflowConsumer> _logger;
        private readonly IActivityService _activityService;

        public StartWorkflowConsumer(ILogger<StartWorkflowConsumer> logger, IActivityService activityService)
        {
            _logger = logger;
            _activityService = activityService;
        }

        public async Task Consume(ConsumeContext<IStartWorkflow> context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            _logger.LogInformation(context.CorrelationId, "StartWorkflowConsumer: got {name}", context.Message.ProfileId);

            await _activityService.StartWorkflow(context.Message).ConfigureAwait(false);

            await context.RespondAsync<IStartWorkflowResponse>(new StartWorkflowResponse { Started = true })
                .ConfigureAwait(false);
        }

    }
}
