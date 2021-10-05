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
    class CustomLog : ILog
    {
        private string _name;
        private ILogger<RepositoryInstaller> _logger;

        public CustomLog(string name, ILogger<RepositoryInstaller> debugLogger)
        {
            _name = name;
            _logger = debugLogger;
        }

        public void Debug(string message)
        {
            _logger.LogDebug($"{_name}: {message}");
        }

        public void Debug(string message, Exception exception)
        {
            _logger.LogDebug(exception,$"{_name}: {message}");
        }

        public void Error(string message)
        {
            _logger.LogError($"{_name}: {message}");
        }

        public void Error(string message, Exception exception)
        {
            _logger.LogError(exception, $"{_name}: {message}");
        }

        public void Fatal(string message)
        {
            _logger.LogCritical($"{_name}: {message}");
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.LogCritical(exception, $"{_name}: {message}");
        }

        public void Info(string message)
        {
            _logger.LogInformation($"{_name}: {message}");
        }

        public void Info(string message, Exception exception)
        {
            _logger.LogInformation(exception, $"{_name}: {message}");
        }

        public void Warn(string message)
        {
            _logger.LogWarning($"{_name}: {message}");
        }

        public void Warn(string message, Exception exception)
        {
            _logger.LogWarning(exception, $"{_name}: {message}");
        }
    }

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
                services.AddHostedService<AwsWorkflowActivityService>();


                var section = configuration.GetSection(AwsWorkflowConfiguration.DefaultConfigName);
                var _config = section.Get<AwsWorkflowConfiguration>();

                services.AddSingleton((provider) => new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region)));

                services.AddSingleton((provider) => new Domain(_config.Domain, provider.GetRequiredService< AmazonSimpleWorkflowClient>()));

                services.AddOptions<AwsWorkflowConfiguration>()
                         .Bind(section)
                         .ValidateDataAnnotations();

                services.AddTransient(typeof(PpoProcessorA));
                services.AddTransient(typeof(PpoProcessorB));
                services.AddTransient(typeof(PpoProcessorC));

                services.AddSingleton<IActivityService,ActivityService>();
                services.AddSingleton<AwsWorkflowConfig>();


                Log.Register(type => new CustomLog(type.Name, _debugLogger));

                _debugLogger.LogDebug("Services added.");
            }
            catch (Exception ex)
            {
                _debugLogger.LogError(ex, "Exception occurred while adding DB services.");
            }
        }
    }
}
