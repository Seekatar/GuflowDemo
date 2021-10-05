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
    public class DeleteWorkflow3Consumer : IConsumer<IDeleteWorkflow3>
    {
        private readonly ILogger<DeleteWorkflow3Consumer> _logger;
        private readonly IWorkflow3Repository _workflow3Repository;

        public DeleteWorkflow3Consumer(ILogger<DeleteWorkflow3Consumer> logger, IWorkflow3Repository Workflow3Repository)
        {
            _logger = logger;
            _workflow3Repository = Workflow3Repository;
        }

        public async Task Consume(ConsumeContext<IDeleteWorkflow3> context)
        {
            if (string.IsNullOrEmpty(context?.Message.Id)) { throw new ArgumentException("context.Message.Id should not be null"); }

            await _workflow3Repository.DeleteWorkflow3Async(context.GetIdentity(), context.Message.Id ).ConfigureAwait(false);
            await context.Publish<IWorkflow3Deleted>(new
            {
                context.Message.Id,
                context.CorrelationId
            }
            ).ConfigureAwait(false);

            _logger.LogInformation(context.CorrelationId, "DeleteWorkflow3Consumer: deleted {id}", context.Message.Id);
        }
    }
}
