using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;

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
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorA"));

            ScheduleActivity<PpoProcessorB>()
                .AfterActivity<PpoProcessorA>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"));

            ScheduleActivity<PpoProcessorC>()
                .AfterActivity<PpoProcessorB>()
                .When(_ => Signal("Signal-PpoProcessorB").IsReceived(d => true))
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"));

            // added this because after C waits forever, even though signaled
            ScheduleActivity<PpoEnd>()
                .AfterActivity<PpoProcessorC>();
        }
    }
}
