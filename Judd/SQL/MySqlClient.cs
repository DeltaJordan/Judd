using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judd.SQL
{
    public static class MySqlClient
    {
        public static MySqlConnection MySqlConnection;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static MySqlClient()
        {
            InitSqlClient();
        }

        private static void InitSqlClient()
        {
            string connectionString =
                $"server={Globals.BotSettings.MySqlIp};user={Globals.BotSettings.MySqlUsername};database=sys;port=3306;password={Globals.BotSettings.MySqlPassword}";

            MySqlConnection = new MySqlConnection(connectionString);
            MySqlConnection.Open();
        }

        private static bool IsConnectionOpen()
        {
            return MySqlConnection?.State == ConnectionState.Open;
        }

        private static bool RefreshConnection()
        {
            try
            {
                MySqlConnection?.Dispose();
                InitSqlClient();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }

            return true;
        }

        public static bool VerifyConnection()
        {
            if (!IsConnectionOpen())
                return RefreshConnection();
            else
                return true;
        }
    }
}
