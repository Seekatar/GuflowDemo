using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using System;

namespace CCC.CAS.Workflow3Service.Workflows
{

    [WorkflowDescription("1.3", DefaultChildPolicy = ChildPolicy.Terminate,
        DefaultTaskListName = "defaultTaskList",
        DefaultExecutionStartToCloseTimeoutInSeconds = 10000,
        DefaultTaskStartToCloseTimeoutInSeconds = 20)]
    public class MainWorkflow : Workflow
    {
        public MainWorkflow()
        {
            var desc = WorkflowDescription.FindOn<PpoWorkflowSignaledDynamic>();

            ScheduleChildWorkflow(desc.Name, desc.Version)
                .OnCompletion(_ => { Console.WriteLine(">>>>>>> All done!!"); return CompleteWorkflow("ok"); });

        }
    }
}
