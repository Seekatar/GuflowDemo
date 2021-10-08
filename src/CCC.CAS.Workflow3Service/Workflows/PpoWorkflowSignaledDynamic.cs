using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;
using Guflow.Worker;
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
        }

        [WorkflowEvent(EventName.WorkflowStarted)]
        public WorkflowAction WorkflowStarted(WorkflowEvent e)
        {
            Console.WriteLine($"In wf started {e} {Activities}");

            WorkflowItems workflowItems = new WorkflowItems();

            //1 error since no default task list
            // Identity identity = Identity.New("PpoProcessorA", "1.0");
            // var acitivityItem = new ActivityItem(identity, this);

            //2 this runs it, but since _workflowItems empty, at end of activity, it fails
            //var description = ActivityDescription.FindOn<PpoProcessorA>();
            //var activityItem = new ActivityItem(Identity.New(description.Name, description.Version), this);
            //workflowItems.Add(activityItem);
            // return  new StartWorkflowAction( workflowItems );

            // 3 this works first time, but throws second time since PpoProcessorA in the list. 
            ScheduleActivity<PpoProcessorA>();

            return StartWorkflow();
        }

    }
}
