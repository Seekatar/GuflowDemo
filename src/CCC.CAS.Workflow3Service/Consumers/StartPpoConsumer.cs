using CCC.CAS.Workflow3Messages.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CCC.CAS.API.Common.ServiceBus;
using CCC.CAS.Workflow3Service.Interfaces;
using System.Web;
using CCC.CAS.API.Common.Logging;
using CCC.CAS.Workflow3Messages.Models;

namespace CCC.CAS.Workflow3Service.Consumers
{
    public class StartPpoConsumer : IConsumer<IStartPpo>
    {
        private readonly ILogger<StartPpoConsumer> _logger;
        private readonly IActivityService _workflow2Repository;

        public StartPpoConsumer(ILogger<StartPpoConsumer> logger, IActivityService activityService)
        {
            _logger = logger;
            _workflow2Repository = activityService;
        }

        public async Task Consume(ConsumeContext<IStartPpo> context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            _logger.LogInformation(context.CorrelationId, "StartWorkflowConsumer: got {name}", context.Message.ProfileId);

            // name shouldn't be anything too wacky since we pass it in the url's path
            await _workflow2Repository.StartWorkflow(context.Message.ProfileId).ConfigureAwait(false);

            await context.RespondAsync<IStartPpoResponse>(new StartPpoResponse { })
                .ConfigureAwait(false);
        }

    }
}
