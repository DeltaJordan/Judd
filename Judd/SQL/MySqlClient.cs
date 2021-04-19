using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.SQL
{
    public static class MySqlClient
    {
        static MySqlClient()
        {
            string connectionString =
                $"server={Globals.BotSettings.MySqlIp};user={Globals.BotSettings.MySqlUsername};database=sdl;port=3306;password={Globals.BotSettings.MySqlPassword}";

            //mySqlConnection = new MySqlConnection(connectionString);
            //mySqlConnection.Open();
        }

    }
}
