using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface ISaveWorkflow3Response : CorrelatedBy<Guid>
    {
        Workflow3? Workflow3 { get;  }
    }

    public class SaveWorkflow3Response : ISaveWorkflow3Response
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public Workflow3? Workflow3 { get; set;  }
    }
}
