using System;
using System.ComponentModel.DataAnnotations;
using Amazon.SimpleWorkflow;
using Microsoft.Extensions.Configuration;

namespace CCC.CAS.AwsWorkflow
{
    public class AwsWorkflowOptions
    {
        // this was here for Spike that did multiple S3 NuGet packages
        // not sure I like this here
        public AmazonSimpleWorkflowClient? GetWf()
        {
            var s3 = new AmazonSimpleWorkflowClient(AccessKey, SecretKey, Amazon.RegionEndpoint.GetBySystemName(Region));
            return s3;
        }

        public const string DefaultConfigName = "Workflow";

        public string ConfigName { get; set; } = DefaultConfigName;

        public static AwsWorkflowOptions Load(IConfiguration configuration, string configName = DefaultConfigName)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            var section = configuration.GetSection(configName);
            return section.Get<AwsWorkflowOptions>();
        }

        [MinLength(5)]
        public string AccessKey { get; set; } = "";
        [MinLength(5)]
        public string SecretKey { get; set; } = "";
        [MinLength(5)]
        public string Region { get; set; } = "";
        [MinLength(5)]
        public string Domain { get; set; } = "";
        [MinLength(5)]
        public string DefaultTaskList { get; set; } = "defaultTaskList";
        [MinLength(5)]
        public string InstallerName { get; set; } = "";

        public bool IsEnabled => !string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(SecretKey) && !string.IsNullOrEmpty(Region) && !string.IsNullOrEmpty(Domain) && !string.IsNullOrEmpty(InstallerName);

        public override string ToString()
        {
            return $@"{nameof(AwsWorkflowOptions)}:
    {nameof(ConfigName)}:           '{ConfigName}'
    {nameof(IsEnabled)}:            '{IsEnabled}'
    {nameof(AccessKey)}:            '{AccessKey}'
    {nameof(SecretKey)}:            '{new string('*', SecretKey.Length)}'
    {nameof(Domain)}:               '{Domain}'
    {nameof(Region)}:               '{Region}'
    {nameof(DefaultTaskList)}:      '{DefaultTaskList}'
";
        }
    }
}