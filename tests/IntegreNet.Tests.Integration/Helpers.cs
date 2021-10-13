using IntegreNet.Model;
using Npgsql;

namespace IntegreNet.Tests.Integration
{
    internal static class Helpers
    {
        public static string CreateConnectionString(Template template)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = template.Database.Config.Port,
                Username = template.Database.Config.Username,
                Password = template.Database.Config.Password,
                Database = template.Database.Config.Database,
                Pooling = false // turn pooling off so connections are truly closed, otherwise IntegreSQL will detect open connections and fail to return a test database
            };

            return builder.ToString();
        }
    }
}
