using System.Text.Json.Serialization;

namespace IntegreNet.Model
{
    public class Database
    {
        [JsonPropertyName("templateHash")]
        public string Hash { get; set; }

        public DatabaseConfig Config { get; set; }
    }
}