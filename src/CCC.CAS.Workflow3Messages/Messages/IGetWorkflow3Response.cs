using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IGetWorkflow3Response : CorrelatedBy<Guid>
    {
        Workflow3? Workflow3 { get;  }
    }

    public class GetWorkflow3Response : IGetWorkflow3Response
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public Workflow3? Workflow3 { get; set;  }
    }
}
