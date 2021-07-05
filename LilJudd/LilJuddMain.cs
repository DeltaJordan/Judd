using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using LilJudd.Support;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LilJudd
{
    public static class LilJuddMain
    {
        public static DiscordClient Client;

        private static CommandsNextExtension commands;
        private static InteractivityExtension interactivity;

        public static async Task Main(string[] args)
        {
            // Make sure Log folder exists
            Directory.CreateDirectory(Path.Combine(Globals.AppPath, "Logs"));

            // Checks for existing latest log
            if (File.Exists(Path.Combine(Globals.AppPath, "Logs", "latest.log")))
            {
                // This is no longer the latest log; move to backlogs
                string oldLogFileName = File.ReadAllLines(Path.Combine(Globals.AppPath, "Logs", "latest.log"))[0];
                File.Move(Path.Combine(Globals.AppPath, "Logs", "latest.log"), Path.Combine(Globals.AppPath, "Logs", oldLogFileName));
            }

            // Builds a file name to prepare for future backlogging
            string logFileName = $"{DateTime.Now:dd-MM-yy}-1.log";

            // Loops until the log file doesn't exist
            int index = 2;
            while (File.Exists(Path.Combine(Globals.AppPath, "Logs", logFileName)))
            {
                logFileName = $"{DateTime.Now:dd-MM-yy}-{index}.log";
                index++;
            }

            // Logs the future backlog file name
            File.WriteAllText(Path.Combine(Globals.AppPath, "Logs", "latest.log"), $"{logFileName}\n");

            // Set up logging through NLog
            LoggingConfiguration config = new LoggingConfiguration();

            FileTarget logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(Globals.AppPath, "Logs", "latest.log"),
                Layout = "[${time}] [${level:uppercase=true}] [${logger}] ${message}"
            };
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);


            ColoredConsoleTarget coloredConsoleTarget = new ColoredConsoleTarget
            {
                UseDefaultRowHighlightingRules = true
            };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, coloredConsoleTarget);
            LogManager.Configuration = config;

            string settingsLocation = Path.Combine(Globals.AppPath, "Data", "settings.json");
            string jsonFile = File.ReadAllText(settingsLocation);

            // Load the settings from file, then store it in the globals
            Globals.BotSettings = JsonConvert.DeserializeObject<Settings>(jsonFile);

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Globals.BotSettings.BotToken,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
            });

            commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
#if DEBUG
                StringPrefixes = new string[] { Globals.BotSettings.Prefix + Globals.BotSettings.Prefix },
#else
                StringPrefixes = new string[] { Globals.BotSettings.Prefix },
#endif
                CaseSensitive = false
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            interactivity = Client.UseInteractivity(new InteractivityConfiguration { });

            Client.MessageCreated += Client_MessageCreated;

            await Client.ConnectAsync();

            await HeartbeatMonitor.BeginMonitorAsync();

            await Task.Delay(-1);
        }

        private static async Task Client_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            try
            {
                if (e.Channel.Type != ChannelType.Text)
                {
                    return;
                }

                DiscordMember authorMember = await e.Guild.GetMemberAsync(e.Author.Id);

                if (authorMember.PermissionsIn(e.Channel).HasPermission(Permissions.Administrator))
                {
                    return;
                }

                if (HeartbeatMonitor.IsJuddDown && e.Message.GetStringPrefixLength("%") > 0)
                {
                    await e.Channel.SendMessageAsync("Judd is currently having issues with processing commands. " +
                        "The developers have been notified of this error and are working to fix this as quickly as possible. Thank you.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
