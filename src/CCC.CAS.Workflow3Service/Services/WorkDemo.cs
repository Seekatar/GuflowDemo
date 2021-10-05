using System;
using System.Threading;

namespace CCC.CAS.Workflow3Service.Services
{
    public static class WorkDemo
    {
        public static void WasteTime(
            int workItemId, WorkDemoActivityState workDemoActivityState)
        {
            if (workDemoActivityState == null) throw new ArgumentNullException(nameof(workDemoActivityState));

            workDemoActivityState.WorkCompleted.Add(workItemId);
            Thread.Sleep(1000);
        }
    }
}
