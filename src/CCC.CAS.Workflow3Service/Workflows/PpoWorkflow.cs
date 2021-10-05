using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Workflows
{
    [WorkflowDescription("1.2", DefaultChildPolicy = ChildPolicy.Terminate,
        DefaultTaskListName = "defaultTaskList",
        DefaultExecutionStartToCloseTimeoutInSeconds = 10000,
        DefaultTaskStartToCloseTimeoutInSeconds = 20)]
    public class PpoWorkflow : Workflow
    {
        public PpoWorkflow()
        {
            ScheduleActivity<PpoProcessorA>();
        }
    }
}
