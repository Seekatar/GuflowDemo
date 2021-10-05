using System;
using System.Collections.Generic;
using Amazon.SimpleWorkflow;

namespace CCC.CAS.Workflow3Service.Services
{
    public class WorkDemoActivityState
    {
        public DateTime EventTimestamp { get; set; }
        public EventType EventType { get; set; } = EventType.ActivityTaskCanceled;
        public int ScenarioNumber { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<int> WorkRequested { get; init; } = new List<int>();
        public List<int> WorkCompleted { get; init; } = new List<int>();
#pragma warning restore CA2227 // Collection properties should be read only

        public override string ToString()
        {
            return $"ScenarioNumber {ScenarioNumber}: " +
                   $"Work Requested: {string.Join("|", WorkRequested.ToArray())} " +
                   $"Work Completed: {string.Join("|", WorkCompleted.ToArray())}";
        }
    }
}
