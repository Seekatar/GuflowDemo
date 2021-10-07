using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using System.Linq;

namespace CCC.CAS.Workflow3Service.Workflows
{
    [WorkflowDescription("1.3", DefaultChildPolicy = ChildPolicy.Terminate,
        DefaultTaskListName = "defaultTaskList",
        DefaultExecutionStartToCloseTimeoutInSeconds = 10000,
        DefaultTaskStartToCloseTimeoutInSeconds = 20)]
    public class PpoWorkflowSignaled : Workflow
    {
        public PpoWorkflowSignaled()
        {
            ScheduleActivity<PpoProcessorA>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorA"))
                ;

            ScheduleActivity<PpoProcessorB>()
//                .When(_ => Signal("Signal-PpoProcessorA").IsTriggered())
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"))
                ;

            //ScheduleActivity<PpoProcessorC>()
            //    .AfterActivity<PpoProcessorB>()
            //    .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"));

            //// added this because after C waits forever, even though signaled
            //ScheduleActivity<PpoEnd>()
            //    .When(a => a.ParentActivity().Result<StartPpoResponse>().Started)
            //    .AfterActivity<PpoProcessorC>();
        }

        [SignalEvent(Name = "Signal-PpoProcessorQ")]
        public WorkflowAction OnSignal(WorkflowSignaledEvent e)
        {
            System.Console.WriteLine(e);

            // does nothing
            ScheduleActivity<PpoProcessorB>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"));

            // doesn't work since "PpoProcessorB" isn't running. We need to remove it from the graph
            //return CancelRequest.For(WorkflowItems.Where(i => (i as IActivityItem)?.Name == "PpoProcessorB"));

            return Ignore; 
        }

    }
}
