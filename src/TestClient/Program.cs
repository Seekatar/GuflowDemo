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
        /// <param name="clientCode">defaults to geico</param>
        /// <param name="profileId">defaults to a geico profileId</param>
        static async Task Main(bool showBusConfig = false, string clientCode = "geico", int profileId = 176453)
        {
            var sender = Sender.Initialize(showBusConfig);
            if (sender == null) { throw new Exception("Didn't get sender!"); }

            var identity = new CallerIdentity { ClientCode = clientCode, ClientProfileId = profileId, Username = "testClient" };

            WriteLine("This is a test client for the message-driven Workflow3 application.");
            await Task.Run(async () =>
            {
                var key = string.Empty;
                while (key != "Q")
                {
                    try
                    {
                        if (int.TryParse(key, out var i))
                        {
                            var gotIt = await sender.StartPpo(new StartPpo { ProfileId = i }, identity);
                            if (gotIt != null)
                            {
                                WriteLine($"Waited for start ppo! Got response '{gotIt.Parm.Message}'");
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
                    WriteLine("Press a number for test scenrio or (Q)uit");
                    key = ReadKey(true).KeyChar.ToString().ToUpperInvariant();
                    WriteLine($"Processing {key}");
                }
            });
        }
    }
}
