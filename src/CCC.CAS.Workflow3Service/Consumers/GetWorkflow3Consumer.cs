using CCC.CAS.Workflow3Messages.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CCC.CAS.API.Common.ServiceBus;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.API.Common.Logging;

namespace CCC.CAS.Workflow3Service.Consumers
{
    public class GetWorkflow3Consumer : IConsumer<IGetWorkflow3>
    {
        private readonly ILogger<GetWorkflow3Consumer> _logger;
        private readonly IWorkflow3Repository _workflow3Repository;

        public GetWorkflow3Consumer(ILogger<GetWorkflow3Consumer> logger, IWorkflow3Repository Workflow3Repository)
        {
            _logger = logger;
            _workflow3Repository = Workflow3Repository;
        }

        public async Task Consume(ConsumeContext<IGetWorkflow3> context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            var result = await _workflow3Repository.GetWorkflow3Async(context.GetIdentity(), context.Message.Id).ConfigureAwait(false);

            _logger.LogInformation(context.CorrelationId, "GetWorkflow3Consumer: got {name}", result?.Name ?? "<not found>");

            await context.RespondAsync<IGetWorkflow3Response>(new GetWorkflow3Response { Workflow3 = result })
                .ConfigureAwait(false);
        }
    }
}
