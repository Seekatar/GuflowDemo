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
    public class SaveWorkflow3Consumer : IConsumer<ISaveWorkflow3>
    {
        private readonly ILogger<SaveWorkflow3Consumer> _logger;
        private readonly IWorkflow3Repository _workflow3Repository;

        public SaveWorkflow3Consumer(ILogger<SaveWorkflow3Consumer> logger, IWorkflow3Repository Workflow3Repository)
        {
            _logger = logger;
            _workflow3Repository = Workflow3Repository;
        }

        public async Task Consume(ConsumeContext<ISaveWorkflow3> context)
        {
            if (string.IsNullOrEmpty(context?.Message.Workflow3?.Name)) { throw new ArgumentException("context.Message.Workflow3.Name should not be null"); }

            var item = await _workflow3Repository.SaveWorkflow3Async(context.GetIdentity(), context.Message.Workflow3, context.CorrelationId ).ConfigureAwait(false);


            if (item != null)
            {
                _logger.LogInformation(context.CorrelationId, "SaveWorkflow3Consumer: saved {name}", context.Message.Workflow3.Name);

                await context.RespondAsync<ISaveWorkflow3Response>(new SaveWorkflow3Response() { Workflow3 = item }).ConfigureAwait(false);
                await context.Publish<IWorkflow3Saved>(new
                {
                    item.Id,
                    item.Name,
                    context.CorrelationId
                }
                ).ConfigureAwait(false);
            }
            else
            {
                await context.RespondAsync<ISaveWorkflow3Response>(new SaveWorkflow3Response()).ConfigureAwait(false);
                _logger.LogError(context.CorrelationId, "Error saving Workflow3. Could publish SaveFailed event");
            }
        }
    }
}
