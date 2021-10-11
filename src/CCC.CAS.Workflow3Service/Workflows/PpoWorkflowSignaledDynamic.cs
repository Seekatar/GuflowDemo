using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using Guflow.Worker;
using System;
using System.Linq;
using System.Text.Json;

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
            ScheduleActivity<PpoProcessorA>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorA"));

            ScheduleActivity<PpoProcessorB>()
                .When(_ => false)
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"));

            ScheduleActivity<PpoProcessorC>()
                .When(_ => false)
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"));

            //    .AfterActivity<PpoProcessorA>()

            //ScheduleActivity<PpoProcessorC>()
            //    .AfterActivity<PpoProcessorB>()
            //    .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"));

            // added this because after C waits forever, even though signaled
            ScheduleActivity<PpoEnd>()
                .AfterActivity<PpoProcessorA>()
                .When(_ => false);
        }

        [SignalEvent(Name = "Signal-PpoProcessorA")]
        public WorkflowAction SignalA(WorkflowSignaledEvent e)
        {
            if (e != null)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<PpoResult>(e.Input);
                    if (result?.Processed ?? false)
                    {
                        return CompleteWorkflow(result.PpoName);
                    }
                    else
                    {
                        return ScheduleWorkflowItemAction.ScheduleByIgnoringWhen((this as IWorkflow).WorkflowItem(Identity.New("PpoProcessorB","1.4")));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Ignore;
        }


        [SignalEvent(Name = "Signal-PpoProcessorB")]
        public WorkflowAction SignalB(WorkflowSignaledEvent e)
        {
            if (e != null)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<PpoResult>(e.Input);
                    if (result?.Processed ?? false)
                    {
                        return CompleteWorkflow(result.PpoName);
                    }
                    else
                    {
                        return ScheduleWorkflowItemAction.ScheduleByIgnoringWhen((this as IWorkflow).WorkflowItem(Identity.New("PpoEnd", "1.3")));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Ignore;
        }
    }
}
