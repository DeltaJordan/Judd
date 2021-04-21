using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Judd.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.Commands
{
    public class RegistrationModule : BaseSDLCommandModule
    {
        private MySqlConnection mySqlConnection = MySqlClient.MySqlConnection;

        [Command("Register")]
        public async Task ApplyForRegistration(CommandContext ctx)
        {
            //get interactivity from user
            // InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            //if the sql server is not responding (error is handled elsewhere)
            if (SQLError)
            {
                return;
            }
            
            //if the user dm'd the bot
            if (ctx.Channel.IsPrivate)
            {
                //if user does not have profile
                if (!IsUserRegistered(ctx.User.Id))
                {
                    //Begin registration and Display users rules
                    await ctx.RespondAsync("yo whatup, ty for ur interest. " +
                        "By continuing you acknowledge that you have read and understand the rules of the server." +
                        "If you agree and would like to continue your registration," +
                        "then please respond: Yes"); //TODO: add cancel at any time
                    //ask for profile info: 

                    //IGN, Nickname, FC
                    //extra: Weapons, Bio, Team, idk whatever
                    //Validation:
                    //is valid name 
                    //is valid FC
                    //approval for info tracking idk
                    //connect to splatnet
                    //void CreateTempProfile (player data);
                    //display profile 
                    //user confirmation
                    // void CreateSQLProfile (player data);      
                    // void CreateDiscordProfileFromSQL; 
                    await ctx.RespondAsync($"{ctx.User} Not registered ");
                    //INSERT INTO `sys`.`Players` (`discordId`, `nickname`, `friend_code`, `power`) VALUES ('200066011616116737', 'Davfernape', '1', '1');
                }
                //else (user has profile) 
                else
                {
                    //tell them they are already registered
                    await ctx.RespondAsync($"{ctx.User} Is already registered");
                }
            }
            //else (the command was NOT in dms where it belongs)
            else
            {
                //tell them they should dm
                await ctx.RespondAsync($"{ctx.User} read the rules");
            }

            // void CreateSQLProfile (player data);      
            // void CreateDiscordProfileFromSQL; 
            // void CreateTempProfile (player data);

        }
         
        private bool IsUserRegistered(ulong discordId)
        {
            long test = 0;
            try
            {
                MySqlCommand command = new MySqlCommand(
                    "SELECT EXISTS( SELECT * FROM Players " +
                    "WHERE discordId=@id)",
                    mySqlConnection);

                command.Parameters.AddWithValue("@id", discordId);
                test = (long)command.ExecuteScalar();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return test > 0;
        }

        //////////////
        ////FC CHECK
        //////////////
            /*code = code.Replace("SW-", string.Empty);

            if (code.Split('-').Length != 3 || code.Split('-').Any(e => !int.TryParse(e, out int _) || e.Length != 4))
            {
                await ctx.RespondAsync("Please use the format 0000-0000-0000!");
                return;
            }*/
    }
}
