using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IStartWorkflowResponse : CorrelatedBy<Guid>
    {
        bool Started { get; }
    }

    public class StartWorkflowResponse : IStartWorkflowResponse
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public bool Started { get; set; }
    }
}
