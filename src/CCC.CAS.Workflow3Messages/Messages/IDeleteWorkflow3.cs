using CCC.CAS.API.Common.ServiceBus;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IDeleteWorkflow3 : ICommandMessage
    {
        // the consumer class name will be '<queue name>Consumer', e.g. DeleteWorkflow3Consumer
        public static Uri QueueUri => new Uri("queue:DeleteWorkflow3?durable=true");

        string Id { get; }
    }

    public class DeleteWorkflow3 : IDeleteWorkflow3
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public Uri MessageQueueUri => IDeleteWorkflow3.QueueUri;

        public string Id { get; set; } = "";
    }
}
