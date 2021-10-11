using System;
using Amazon;
using Amazon.SimpleWorkflow;
using CCC.CAS.API.Common.Installers;
using CCC.CAS.API.Common.Logging;
using CCC.CAS.Workflow3Service.Activities;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.Workflow3Service.Repositories;
using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CCC.CAS.Workflow3Service.Installers
{
    public class RepositoryInstaller : IInstaller
    {
        private readonly ILogger<RepositoryInstaller> _debugLogger;

        public RepositoryInstaller()
        {
            _debugLogger = DebuggingLoggerFactory.Create<RepositoryInstaller>();
        }

        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            try
            {
                services.AddHostedService<AwsWorkflowDeciderService>();

                var section = configuration.GetSection(AwsWorkflowOptions.DefaultConfigName);
                var _config = section.Get<AwsWorkflowOptions>();

                services.AddSingleton((provider) => new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region)));

                services.AddSingleton((provider) => new Domain(_config.Domain, provider.GetRequiredService<AmazonSimpleWorkflowClient>()));

                services.AddOptions<AwsWorkflowOptions>()
                         .Bind(section)
                         .ValidateDataAnnotations();


                services.AddSingleton<IActivityService,ActivityService>();
                services.AddSingleton<AwsWorkflowRegistration>();

                Log.Register(type => new GuflowLogger(type.Name, _debugLogger));

                _debugLogger.LogDebug("Services added.");
            }
            catch (Exception ex)
            {
                _debugLogger.LogError(ex, "Exception occurred while adding DB services.");
            }
        }
    }
}
