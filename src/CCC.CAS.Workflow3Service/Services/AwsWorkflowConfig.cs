using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleWorkflow;
using Amazon;
using Amazon.SimpleWorkflow.Model;
using Microsoft.Extensions.Logging;

namespace CCC.CAS.Workflow3Service.Services
{
    public class AwsWorkflowConfig
    {
        private readonly AwsWorkflowConfiguration _config;
        private readonly ILogger<AwsWorkflowConfig> _logger;

        public AwsWorkflowConfig(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowConfig> logger)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.Value;
            _logger = logger;
        }

        private async Task<ActivityTypeInfo?> GetActivity(AmazonSimpleWorkflowClient client, string activityName)
        {
            var listRequest = new ListActivityTypesRequest
            {
                Name = activityName,
                Domain = _config.Domain,
                MaximumPageSize = 1,
                RegistrationStatus = RegistrationStatus.REGISTERED
            };

            var response = await client.ListActivityTypesAsync(listRequest).ConfigureAwait(false);
            return response.ActivityTypeInfos.TypeInfos.FirstOrDefault();
        }

        public async Task Register()
        {
            using var client = new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region));

            string[] activityNames = { "DemoActivity1", "DemoActivity2", "DemoActivity3", "DemoActivity4" };
            string version = "1.3";

            foreach (var name in activityNames )
            {
                if (await GetActivity(client, name).ConfigureAwait(false) == null)
                {
                    RegisterActivityTypeRequest request = new()
                    {
                        DefaultTaskList = new TaskList() { Name = _config.DefaultTaskList },
                        DefaultTaskScheduleToCloseTimeout = "600",
                        DefaultTaskScheduleToStartTimeout = "600",
                        Domain = _config.Domain,
                        DefaultTaskStartToCloseTimeout = "600",
                        Name = name,
                        Version = version,
                        DefaultTaskHeartbeatTimeout = "NONE"
                    };
                    try
                    {
                        await client.RegisterActivityTypeAsync(request).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error registering {name} {version}");
                    }
                }
            }
        }
    }
}
