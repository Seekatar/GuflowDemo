using CCC.CAS.API.Common.ServiceBus;

namespace CCC.CAS.Workflow3Service.Installers
{
    public class Workflow3ActiveMqConfiguration : ActiveMqConfiguration
    {
        public static string SaveWorkflow3Endpoint => "queue:SaveWorkflow3?durable=true";
        public static string Workflow3SavedEndpoint => "topic:Workflow3Saved?durable=true";
    }
}
