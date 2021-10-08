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
    public class PpoResult
    {
        public string PpoName { get; set; } = "";
        public bool Processed { get; set; }
    }

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

            await DoWork(args, startPpo).ConfigureAwait(false);

            return Complete(new { Started = true });
        }

        private async Task DoWork(ActivityArgs args, IStartPpo startPpo)
        {
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            if (SendSignal())
            {
                var _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

                    string workflowId = args.WorkflowId;
                    string runId = args.WorkflowRunId;

                    await Domain.SignalWorkflowAsync(new SignalWorkflowRequest(workflowId, $"Signal-{GetType().Name}")
                    {
                        SignalInput = new PpoResult { PpoName = GetType().Name, Processed = Processed() },
                        WorkflowRunId = runId
                    }).ConfigureAwait(false);
                    Logger.LogInformation(">>>>>>>>>> {typeName} signaled for {clientCode}", GetType().Name, startPpo.ClientCode);

                });
            }
        }

        virtual protected bool Processed()
        {
            return true;
        }

        virtual protected bool SendSignal()
        {
            return true;
        }
    }
}