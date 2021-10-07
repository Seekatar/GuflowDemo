using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Activities
{
    public class PpoProcessor<T> : CasActvity<T> where T : class
    {
        public PpoProcessor(IOptions<AwsWorkflowOptions> config, ILogger<T> logger, Domain domain) : base(config, logger, domain)
        {
        }

        [ActivityMethod]
        public async Task<ActivityResponse> Execute(ActivityArgs args)
        {
            var startPpo = args?.Input.As<StartPpo>() ?? throw new ArgumentNullException(nameof(args));

            Logger.LogInformation(">>>>>>>>>> {typeName} processing for {clientCode}", GetType().Name, startPpo.ClientCode);

            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            if (!String.Equals(startPpo.ClientCode, "nationwide", StringComparison.OrdinalIgnoreCase))
            {
                DoWork(args, startPpo);
            }

            return Complete(new { Started = true });
        }

        private void DoWork(ActivityArgs args, IStartPpo startPpo)
        {
            var _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

                string workflowId = args.WorkflowId;
                string runId = args.WorkflowRunId;

                await Domain.SignalWorkflowAsync(new SignalWorkflowRequest(workflowId, $"Signal-{GetType().Name}")
                {
                    SignalInput = new { Test = 1, ClassName = GetType().Name },
                    WorkflowRunId = runId
                }).ConfigureAwait(false);
                Logger.LogInformation(">>>>>>>>>> {typeName} signaled for {clientCode}", GetType().Name, startPpo.ClientCode);

            });
        }
    }
}