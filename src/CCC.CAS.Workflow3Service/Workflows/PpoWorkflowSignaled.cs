using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using System;
using System.Text.Json;

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
                .When(_ => Signal("Signal-PpoProcessorB").IsReceived(data =>
                            { try
                                {
                                    var startPop = JsonSerializer.Deserialize<StartPpo>(data);
                                    return startPop?.PpoConsumer == this.GetType().Name;
                                } 
                                catch(Exception e)
                                {
                                    Console.WriteLine(e);
                                    return true;
                                }
                            } ))
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"));

            // added this because after C waits forever, even though signaled
            ScheduleActivity<PpoEnd>()
                .AfterActivity<PpoProcessorC>();
        }
    }
}
