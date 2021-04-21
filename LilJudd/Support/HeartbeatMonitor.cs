using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LilJudd.Support
{
    public static class HeartbeatMonitor
    {
        public static bool IsJuddDown { get; private set; }

        private static DateTime lastHeartbeat;
        private static bool continueMonitor;

        public static async Task BeginMonitorAsync()
        {
            continueMonitor = true;

            await MonitorAsync();
        }

        public static void EndMonitor()
        {
            continueMonitor = false;
        }

        private static async Task MonitorAsync()
        {
            string botHeartbeatFile = Path.Combine(Globals.AppPath, "Data", "JuddLifeSupport.json");

            using FileStream fileStream = new FileStream(botHeartbeatFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream);

            IsJuddDown = false;

            while (continueMonitor)
            {
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                string heartbeatText = await streamReader.ReadToEndAsync();

                lastHeartbeat = JsonConvert.DeserializeObject<DateTime>(heartbeatText);

                TimeSpan timeDifference = DateTime.Now - lastHeartbeat;

                if (timeDifference.TotalMinutes > 2)
                {
                    if (!IsJuddDown)
                    {
                        await HandleDownAsync();

                        IsJuddDown = true;
                    }
                }
                else
                {
                    IsJuddDown = false;
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        private static async Task HandleDownAsync()
        {
            DiscordGuild sdlGuild = await LilJuddMain.Client.GetGuildAsync(832018934503964673);
            DiscordChannel messageChannel = sdlGuild.GetChannel(833585556637483008);

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Judd is currently down.",
                Color = DiscordColor.Red
            };

            builder.AddField("Last Heartbeat", lastHeartbeat.ToLongTimeString());
            builder.WithFooter("To continue to check the last heartbeat, use %heartbeat.");

            await messageChannel.SendMessageAsync(embed: builder.Build());
        }
    }
}
