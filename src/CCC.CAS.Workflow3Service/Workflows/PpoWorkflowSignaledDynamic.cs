using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using System;
using System.Linq;

namespace CCC.CAS.Workflow3Service.Workflows
{
    [WorkflowDescription("1.3", DefaultChildPolicy = ChildPolicy.Terminate,
        DefaultTaskListName = "defaultTaskList",
        DefaultExecutionStartToCloseTimeoutInSeconds = 10000,
        DefaultTaskStartToCloseTimeoutInSeconds = 20)]
    public class PpoWorkflowSignaledDynamic : Workflow
    {
        public PpoWorkflowSignaledDynamic()
        {
            Started += OnWorkflowStarted;
        }

        private void OnWorkflowStarted(object? sender, WorkflowStartedEventArgs e)
        {
            ScheduleActivity<PpoProcessorB>();
        }


        [SignalEvent(Name = "Signal-PpoProcessorQ")]
        public WorkflowAction OnSignal(WorkflowSignaledEvent e)
        {
            System.Console.WriteLine(e);

            // does nothing
            ScheduleActivity<PpoProcessorB>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"));

            // doesn't work since "PpoProcessorB" isn't running. We need to remove it from the graph
            // return CancelRequest.For(WorkflowItems.Where(i => (i as IActivityItem)?.Name == "PpoProcessorB"));

            return Ignore; 
        }

    }
}
