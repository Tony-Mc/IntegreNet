using System.Text.Json.Serialization;

namespace IntegreNet.Model
{
    /// <summary>
    /// IntegreSQL managed database information.
    /// </summary>
    public sealed class Database
    {
        /// <summary>
        /// Provided hash for which this database was created.
        /// </summary>
        /// <remarks>This allows you to identify the combination of your database migration/fixture files for which this database was created.</remarks>
        [JsonPropertyName("templateHash")]
        public string Hash { get; set; }

        /// <summary>
        /// Database configuration.
        /// </summary>
        public DatabaseConfig Config { get; set; }
    }
}