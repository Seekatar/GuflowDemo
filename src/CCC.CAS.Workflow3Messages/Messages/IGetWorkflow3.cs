using CCC.CAS.API.Common.ServiceBus;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IGetWorkflow3 : ICommandMessage
    {
        // the consumer class name will be '<queue name>Consumer', e.g. GetWorkflow3Consumer
        public static Uri QueueUri => new Uri("queue:GetWorkflow3?durable=true");

        string Id { get; }
    }

    public class GetWorkflow3 : IGetWorkflow3
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public Uri MessageQueueUri => IGetWorkflow3.QueueUri;

        public string Id { get; set; } = "";
    }
}
