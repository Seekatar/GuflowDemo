using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Messages.Models;
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
            string? id = null;

            WriteLine("This is a test client for the message-driven Workflow3 application.");
            await Task.Run(async () =>
            {
                var key = string.Empty;
                while (key != "Q")
                {
                    try
                    {
                        switch (key)
                        {
                            case "E":
                                {
                                    var gotIt = await sender.GetEcho(new GetEcho { Name = "Fred" }, identity);
                                    if (gotIt != null)
                                    {
                                        WriteLine($"Waited for echo! Got response '{gotIt.Parm.Message}'");
                                    }
                                    else
                                    {
                                        WriteLine("Waiting for GetEchoResponse published message timed out!");
                                    }
                                    break;
                                }
                            case "S":
                                {
                                    var workflow3 = await sender.SaveWorkflow3(new SaveWorkflow3 { Workflow3 = new Workflow3 { Name = "ABC123" } }, identity);
                                    if (workflow3 != null)
                                    {
                                        id = workflow3.Id;
                                    }
                                    break;
                                }
                            case "D":
                                {
                                    if (id == null)
                                    {
                                        WriteLine("Must save before calling delete");
                                    }
                                    else
                                    {
                                        await sender.DeleteWorkflow3(new DeleteWorkflow3 { Id = id }, identity);
                                    }
                                    break;
                                }
                            case "P":
                                {
                                    await sender.PublishSaved();
                                    break;
                                }
                            case "G":
                                {
                                    if (id == null)
                                    {
                                        WriteLine("Must save before calling get");
                                    }
                                    else
                                    {
                                        var workflow3 = await sender.GetWorkflow3(new GetWorkflow3 { Id = id }, identity);
                                        if (workflow3 != null)
                                        {
                                            WriteLine($"Got workflow3 with name '{workflow3?.Name}'");
                                        }
                                        else
                                        {
                                            WriteLine($"Didn't get workflow3");

                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLine(e);
                    }
                    WriteLine("Press a key: (S)aveWorkflow3 (G)etWorkflow3 (D)eleteWorkflow3 (P)ublishWorkflow3Saved (E)choTest (Q)uit");
                    key = ReadKey(true).KeyChar.ToString().ToUpperInvariant();
                    WriteLine($"Processing {key}");
                }
            });
        }
    }
}
