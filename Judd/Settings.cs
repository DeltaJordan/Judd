using System.IO;
using Newtonsoft.Json;

namespace Judd
{
    public class Settings
    {
        [JsonProperty("bot_token")]
        public string BotToken { get; set; }

        [JsonProperty("app_key")]
        public string AppKey { get; set; }

        [JsonProperty("base_id")]
        public string BaseId { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("mysql_ip")]
        public string MySqlIp { get; set; }

        [JsonProperty("mysql_user")]
        public string MySqlUsername { get; set; }

        [JsonProperty("mysql_pass")]
        public string MySqlPassword { get; set; }

        public void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(Path.Combine(Globals.AppPath, "Data", "settings.json"), json);
        }
    }
}
