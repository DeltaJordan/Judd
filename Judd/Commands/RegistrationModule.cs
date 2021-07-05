using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Judd.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            //if the sql server is not responding (error is handled elsewhere)
            if (SQLError)
            {
                return;
            }
            
            //if the user dm'd the bot
            if (!ctx.Channel.IsPrivate)
            {
                //if user does not have profile
                if (!IsUserRegistered(ctx.User.Id))
                {
                    ////////////////////
                    //Begin registration and Display users rules
                    ////////////////////
                    DiscordMessage botPrompt = await ctx.RespondAsync("yo whatup, ty for ur interest. " +
                        "By continuing you acknowledge that you have read and understand the rules of the server." +
                        "If you agree and would like to continue your registration," +
                        "then please respond: Yes" +
                        "\n Note: if you would like to cancel at anytime respond with: Cancel" +
                        "\n If you do not respond to any of the prompts within 10 minutes, " +
                        "your registration will be canceled"); //TODO: add cancel at any time

                    InteractivityResult<DiscordMessage> userResponse = await ctx.Message.GetNextMessageAsync(TimeSpan.FromMinutes(10));
                    
                    bool canceled;
                    string botresponse = IsCanceled(userResponse, out canceled);
                    if (canceled)
                    {
                        await ctx.RespondAsync(botresponse);
                        return;
                    }

                    if(userResponse.Result.Content.ToLower() != "yes")
                    {
                        await ctx.RespondAsync("Okay!");
                        return;
                    }

                    ///////////////////////
                    //ask for profile info: 
                    ///////////////////////
                    ///
                    ///////////
                    //IGN
                    //////////
                    botPrompt = await ctx.RespondAsync("Cool, please enter your switch ign. between 1-10 characters");
                    userResponse = await ctx.Message.GetNextMessageAsync(TimeSpan.FromMinutes(10));
                    
                    botresponse = IsCanceled(userResponse, out canceled);
                    if (canceled)
                    {
                        await ctx.RespondAsync(botresponse);
                        return;
                    }

                    string ign = userResponse.Result.Content;
                    if (IsIgnValid(ign))
                    {
                        await ctx.RespondAsync("ur ign is set to " + ign + ", u can change this later");
                    }
                    else
                    {
                        await ctx.RespondAsync("invalid ign");
                        return;
                    }
                    /////////
                    //FC
                    /////////
                    botPrompt = await ctx.RespondAsync("Next, please enter your switch FC using the format: 0000-0000-0000 or SW-0000-0000-0000");
                    userResponse = await ctx.Message.GetNextMessageAsync(TimeSpan.FromMinutes(10));

                    botresponse = IsCanceled(userResponse, out canceled);
                    if (canceled)
                    {
                        await ctx.RespondAsync(botresponse);
                        return;
                    }

                    string FC = userResponse.Result.Content;
                    if (IsFCValid(FC))
                    {
                        await ctx.RespondAsync("ur fc is set to " + FC + ", u can change this later");
                    }
                    else
                    {
                        await ctx.RespondAsync("invalid fc pls use the format: 0000-0000-0000 or SW-0000-0000-0000");
                    }

                    //extra: Weapons, Bio, Team, idk whatever

                    //approval for info tracking idk
                    //connect to splatnet

                    //void CreateTempProfile (player data);
                    //display profile 

                    //user confirmation

                    // void CreateSQLProfile (player data);      
                    // void CreateDiscordProfileFromSQL; 
                    await ctx.RespondAsync($"{ctx.User.Username} you are now registered\n" +
                        $"your ign is: " + ign + $"\n"
                        + $"your fc is: " + FC);
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
                await ctx.RespondAsync($"{ctx.User} please dm me with this command");
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

        private bool IsIgnValid(string ign)
        {
            if (ign.Length > 10)
            {
                return false;
            }

            if (!Regex.IsMatch(ign, "^[a-zA-Z0-9]*$"))
            {
                return false;
            }

            return true;
        }

        private bool IsFCValid(string FC)
        {
            FC = FC.Replace("SW-", string.Empty);

            if (FC.Split('-').Length != 3 || FC.Split('-').Any(e => !int.TryParse(e, out int _) || e.Length != 4))
            {
                return false;
            }

            return true;
        }

        
        private string IsCanceled(InteractivityResult<DiscordMessage> userResponse, out bool canceled)
        {
            if (userResponse.TimedOut)
            {
                canceled = true;
                return "You have timed out and your registration has been canceleed, " +
                    "try again when you have time";
            }
            if (userResponse.Result.Content.ToLower() == "cancel")
            {
                canceled = true;
                return "Your registration has been canceled";
            }
            canceled = false;
            return "";
        }
    }
}
