namespace IntegreNet.Model
{
    /// <summary>
    /// Database configuration.
    /// </summary>
    public sealed class DatabaseConfig
    {
        /// <summary>
        /// The hostname of a PostgreSQL database managed by IntegreSQL.
        /// </summary>
        /// <remarks>
        /// Depending on your setup this may not always be a reachable hostname.
        /// </remarks>
        public string Host { get; set; }

        /// <summary>
        /// PostgreSQL database port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// PostgreSQL database user name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// PostgreSQL database user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// PostgreSQL database name.
        /// </summary>
        public string Database { get; set; }
    }
}