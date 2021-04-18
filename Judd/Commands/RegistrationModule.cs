using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.Commands
{
    public class RegistrationModule : BaseCommandModule
    {
        //TASK
            //command trigger (reaction / chat input)

            //if user does not have profile
                //dm
                //rules
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
            //else (user has profile) 
                //ligma balls

            
          // void CreateSQLProfile (player data);      
          // void CreateDiscordProfileFromSQL; 
          // void CreateTempProfile (player data);


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
