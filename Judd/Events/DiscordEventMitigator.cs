using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.Events
{
    public static class DiscordEventMitigator
    {
        public static async Task RegisterEventsAsync(DiscordClient client)
        {
            client.Heartbeated += Client_Heartbeated;
        }

        private static async Task Client_Heartbeated(DiscordClient sender, DSharpPlus.EventArgs.HeartbeatEventArgs e)
        {
            Console.WriteLine($"{e.Timestamp:G}");
        }
    }
}
