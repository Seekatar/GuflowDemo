using System;
using CCC.CAS.API.Common.Logging;
using Guflow;
using Microsoft.Extensions.Logging;

namespace CCC.CAS.Workflow
{
    class GuflowLogger : ILog
    {
        private readonly string _name;
        private readonly ILogger _logger;

        public GuflowLogger(string name, ILogger debugLogger)
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
}
