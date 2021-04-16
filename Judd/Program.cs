using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Judd
{
    public static class Program
    {
        public static DiscordClient Client;

        private static InteractivityExtension interactivity;
        private static CommandsNextExtension commands;

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
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            });

            commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
#if DEBUG_PREFIX
                StringPrefix = Globals.BotSettings.Prefix + Globals.BotSettings.Prefix,
#else
                StringPrefixes = new string[] { Globals.BotSettings.Prefix },
#endif
                CaseSensitive = false
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            interactivity = Client.UseInteractivity(new InteractivityConfiguration { });

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
