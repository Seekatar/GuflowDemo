using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System.Threading.Tasks;
using static System.Console;
using CCC.CAS.API.Common.ServiceBus;
using CCC.CAS.API.Common.Models;
using System;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CCC.CAS.API.AspNetCommon.Extensions;
using MassTransit.ActiveMqTransport;
using System.Text.Json;
using GreenPipes;

namespace IntegrationTest
{
    /// <summary>
    /// class to send commands and events, broken out to show DI working.
    /// </summary>
    public class Sender : IDisposable
    {
        private readonly IBusControl _busControl;
        static private IConfigurationRoot? _configuration;
        static private ServiceProvider? _serviceWorkflow3;
        static readonly object _lock = new();
        static private Sender? _me;
        private static IBusControl? _bus;

        public Sender(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public static Sender Initialize(bool showBusConfig = false)
        {
            lock (_lock)
            {
                if (_me == null)
                {
                    _configuration = new ConfigurationBuilder()
                              .AddSharedSettings()
                              .AddJsonFile("appsettings.json", true, true)
                              .AddEnvironmentVariables()
                              .Build();

                    IServiceCollection serviceCollection = new ServiceCollection();
                    _serviceWorkflow3 = ConfigureServices(_configuration, serviceCollection);
                    if (_serviceWorkflow3 == null) { throw new Exception("Didn't get service provider!"); }

                    _me = _serviceWorkflow3.GetService<Sender>();
                    if (_me == null) { throw new Exception("Didn't get sender!"); }

                    _bus = _serviceWorkflow3.GetRequiredService<IBusControl>();
                    _bus.Start();

                    if (showBusConfig)
                    {
                        var result = _bus.GetProbeResult();
                        WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
                        WriteLine();
                    }
                }
            }

            return _me;
        }

        private static ServiceProvider? ConfigureServices(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            var config = configuration.GetSection("ActiveMq").Get<ActiveMqConfiguration>();
            if (config == null)
            {
                WriteLine("Didn't get ActiveMq config from JSON or env");
                return null;
            }

            serviceCollection.AddMassTransit(svcColl =>
            {
                svcColl.AddConsumers(Assembly.GetExecutingAssembly());

                svcColl.AddBus(provider => Bus.Factory.CreateUsingActiveMq(cfg =>
                {
                    cfg.Host(config.Host, h =>
                    {
                        h.Username(config.Username);
                        h.Password(config.Password);

                    });
                    cfg.Durable = true;
                    cfg.ConfigureEndpoints(provider); // convention-based registration
                }));
            });

            serviceCollection.AddSingleton<Sender>();

            serviceCollection.AddSingleton<IHostedService, BusService>();

            return serviceCollection.BuildServiceProvider();
        }

        // Another example of Request/Response call where consumer sends response to this caller
        public async Task<bool> StartPpo(IStartPpo StartPpo, CallerIdentity identity)
        {
            return (await _busControl.RequestResponse<IStartPpo, IStartPpoResponse>(StartPpo, identity).ConfigureAwait(false))?.Started ?? false;
        }

        public void Dispose()
        {
            _bus?.Stop();
            _serviceWorkflow3?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> StartWorkflow(StartWorkflow startWorkflow, CallerIdentity identity)
        {
            return (await _busControl.RequestResponse<IStartWorkflow, IStartWorkflowResponse>(startWorkflow, identity).ConfigureAwait(false))?.Started ?? false;
        }
    }
}
