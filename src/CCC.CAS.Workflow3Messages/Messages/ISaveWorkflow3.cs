using CCC.CAS.API.Common.ServiceBus;
using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface ISaveWorkflow3 : ICommandMessage
    {
        Workflow3? Workflow3 { get; }

        // the consumer class name will be '<queue name>Consumer', e.g. SaveWorkflow3Consumer
        static Uri QueueUri = new Uri("queue:SaveWorkflow3?durable=true");
    }

    public class SaveWorkflow3 : ISaveWorkflow3
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public Uri MessageQueueUri => ISaveWorkflow3.QueueUri;

        public Workflow3? Workflow3 { get; set; }
    }
}
