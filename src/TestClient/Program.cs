using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Messages;
using System;
using System.Threading.Tasks;
using static System.Console;
using IntegrationTest;

namespace TestMessages
{
    /// <summary>
    /// test program to fire commands, events, and listen for events
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="showBusConfig">if set to <c>true</c> [show bus configuration].</param>
        static async Task Main(bool showBusConfig = false)
        {
            var sender = Sender.Initialize(showBusConfig);
            if (sender == null) { throw new Exception("Didn't get sender!"); }



            var startPpo = new StartPpo[] { new StartPpo
                                            {
                                                ClientCode = "geico",
                                                ProfileId = 176453,
                                                RequestId = Guid.NewGuid().ToString(),
                                                SendSignal = true
                                            },
                                            new StartPpo
                                            {
                                                ClientCode = "nationwide",
                                                ProfileId = 180181,
                                                RequestId = Guid.NewGuid().ToString()
                                            },
                                            new StartPpo
                                            {
                                                ClientCode = "usaa",
                                                ProfileId = 180181,
                                                RequestId = Guid.NewGuid().ToString(),
                                                SendSignal = true,
                                                PpoBConsume = true
                                            }
            };
            WriteLine("This is a test client for the message-driven Workflow application.");
            await Task.Run(async () =>
            {
                var key = string.Empty;
                while (key != "Q")
                {
                    try
                    {
                        if (int.TryParse(key, out var j) && j <= startPpo.Length)
                        {
                            var i = j - 1;
                            startPpo[i].CorrelationId = Guid.NewGuid();
                            var identity = new CallerIdentity { ClientCode = startPpo[i].ClientCode, ClientProfileId = startPpo[i].ProfileId, Username = "testClient" };
                            var started = await sender.StartPpo(startPpo[i], identity);
                            if (started)
                            {
                                WriteLine($"Waited for start ppo for {startPpo[i].ClientCode}!");
                            }
                            else
                            {
                                WriteLine("Waiting for StartPpoResponse published message timed out!");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLine(e);
                    }
                    WriteLine("1 = geico signaled ABC");
                    WriteLine("2 = nw static ACB");
                    WriteLine("3 = usaa dyanmic wf A");
                    WriteLine("(Q)uit");
                    key = ReadKey(true).KeyChar.ToString().ToUpperInvariant();
                    WriteLine($"Processing {key}");
                }
            });
        }
    }
}
