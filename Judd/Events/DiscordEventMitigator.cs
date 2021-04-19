using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.Events
{
    public static class DiscordEventMitigator
    {
        public static Task RegisterEventsAsync(DiscordClient client)
        {
            client.Heartbeated += Client_Heartbeated;
            return Task.CompletedTask;
        }

        private static Task Client_Heartbeated(DiscordClient sender, DSharpPlus.EventArgs.HeartbeatEventArgs e)
        {
            string buildFolder = "Release";
#if DEBUG
            buildFolder = "Debug";
#endif

            string timestamp = JsonConvert.SerializeObject(e.Timestamp);

            DirectoryInfo directoryInfo = Directory.CreateDirectory(Globals.AppPath);

            while (directoryInfo.Name != "Judd")
            {
                directoryInfo = directoryInfo.Parent;
            }

            directoryInfo = directoryInfo.Parent;

            File.WriteAllText(Path.Combine(directoryInfo.FullName,
               "LilJudd", "bin", buildFolder, "net5.0", "Data", "JuddLifeSupport.json"),
               timestamp);

            return Task.CompletedTask;
        }
    }
}
