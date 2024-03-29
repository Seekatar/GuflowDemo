﻿using CCC.CAS.Workflow3Service.Activities;
using Guflow.Decider;

namespace CCC.CAS.Workflow3Service.Workflows
{

    [WorkflowDescription("1.3", DefaultChildPolicy = ChildPolicy.Terminate,
        DefaultTaskListName = "defaultTaskList",
        DefaultExecutionStartToCloseTimeoutInSeconds = 10000,
        DefaultTaskStartToCloseTimeoutInSeconds = 20)]
    public class PpoWorkflow : Workflow
    {
        public PpoWorkflow()
        {
            ScheduleActivity<PpoProcessorA>();

            ScheduleActivity<PpoProcessorC>()
                .AfterActivity<PpoProcessorA>();

            ScheduleActivity<PpoProcessorB>()
                .AfterActivity<PpoProcessorC>();

        }
    }
}
