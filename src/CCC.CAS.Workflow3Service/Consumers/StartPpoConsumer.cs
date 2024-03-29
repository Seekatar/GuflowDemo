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
    public class StartPpoConsumer : IConsumer<IStartPpo>
    {
        private readonly ILogger<StartPpoConsumer> _logger;
        private readonly IWorkflowService _workflow2Repository;

        public StartPpoConsumer(ILogger<StartPpoConsumer> logger, IWorkflowService activityService)
        {
            _logger = logger;
            _workflow2Repository = activityService;
        }

        public async Task Consume(ConsumeContext<IStartPpo> context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            _logger.LogInformation(context.CorrelationId, "StartWorkflowConsumer: got {name}", context.Message.ProfileId);

            // name shouldn't be anything too wacky since we pass it in the url's path
            await _workflow2Repository.StartPpoWorkflow(context.Message).ConfigureAwait(false);

            await context.RespondAsync<IStartPpoResponse>(new StartPpoResponse { Started = true })
                .ConfigureAwait(false);
        }

    }
}
