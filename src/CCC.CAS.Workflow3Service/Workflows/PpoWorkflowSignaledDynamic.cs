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
                .AfterActivity<PpoProcessorA>()
                .OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorB"));

            ScheduleActivity<PpoProcessorC>()
                .AfterActivity<PpoProcessorB>()
                .When(_ => Signal("Signal-PpoProcessorB").IsTriggered(data =>
                {
                    // Jump.ToActivity<PpoEnd>();
                    // return true, executed C
                    // return false, waits
                    try
                    {
                        var startPop = JsonSerializer.Deserialize<StartPpo>(data);
                        Console.WriteLine($"^^^^^^^^ { !startPop?.PpoBConsume}");
                        if (startPop?.PpoBConsume ?? false)
                        {
                            CompleteWorkflow("Ok");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return true;
                    }
                }))
                //.OnCompletion(e => e.WaitForSignal("Signal-PpoProcessorC"))
                ;

            // added this because after C waits forever, even though signaled
            //ScheduleActivity<PpoEnd>()
            //    .AfterActivity<PpoProcessorC>();
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Ignore;
        }

        //[WorkflowEvent(EventName.WorkflowStarted)]
        //public WorkflowAction WorkflowStarted(WorkflowEvent e)
        //{
        //    Console.WriteLine($"In wf started {e} {Activities}");

        //    WorkflowItems workflowItems = new WorkflowItems();

        //    //1 error since no default task list
        //    // Identity identity = Identity.New("PpoProcessorA", "1.0");
        //    // var acitivityItem = new ActivityItem(identity, this);

        //    //2 this runs it, but since _workflowItems empty, at end of activity, it fails
        //    //var description = ActivityDescription.FindOn<PpoProcessorA>();
        //    //var activityItem = new ActivityItem(Identity.New(description.Name, description.Version), this);
        //    //workflowItems.Add(activityItem);
        //    // return  new StartWorkflowAction( workflowItems );

        //    // 3 this works first time, but throws second time since PpoProcessorA in the list. 
        //    ScheduleActivity<PpoProcessorA>();

        //    return StartWorkflow();
        //}

    }
}
