using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Judd.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.Commands
{
    public class BaseSDLCommandModule : BaseCommandModule
    {
        private MySqlConnection mySqlConnection = MySqlClient.MySqlConnection;

        public bool SQLError = false;

        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            if (!MySqlClient.VerifyConnection())
            {
                SQLError = true;
                await HandleServerOutage(ctx);
            }
        }

        private async Task HandleServerOutage(CommandContext ctx)
        {
            await ctx.RespondAsync($"Hey {ctx.User.Username} It dont work rn lmao");
            DiscordChannel channel = ctx.Guild.GetChannel(833585556637483008);
            DiscordRole devRole = ctx.Guild.GetRole(833585225610952715);
            await channel.SendMessageAsync($"{devRole.Mention} SQL Error occured in Message:{ctx.Message.JumpLink}");
        }
    }
}
