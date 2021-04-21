using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilJudd.Commands
{
    public class HeartbeatModule : BaseCommandModule
    {
        [Command("heartbeat")]
        public async Task GetHeartbeatAsync(CommandContext ctx)
        {
            await ctx.RespondAsync(JsonConvert.DeserializeObject<DateTime>(File.ReadAllText(Path.Combine(Globals.AppPath, "Data", "JuddLifeSupport.json"))).ToLongDateString());
        }
    }
}
