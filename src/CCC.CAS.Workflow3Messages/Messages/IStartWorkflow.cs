using CCC.CAS.API.Common.ServiceBus;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IStartWorkflow : ICommandMessage
    {
        public static Uri QueueUri => new("queue:StartWorkflow?durable=true");

        string RequestId { get; }
        string ClientCode { get; }
        int ProfileId { get; }
        bool SendSignal { get; }
        string PpoConsumer { get; }
    }

    public class StartWorkflow : IStartWorkflow
    {
        public Guid CorrelationId { get; set; } = NewId.NextGuid();
        public Uri MessageQueueUri => IStartWorkflow.QueueUri;

        public string RequestId { get; set; } = "";

        public string ClientCode { get; set; } = "";

        public int ProfileId { get; set; }

        public bool SendSignal { get; set; }

        public string PpoConsumer { get; set; } = "";
    }
}
