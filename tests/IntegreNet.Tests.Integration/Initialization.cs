using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NUnit.Framework;

namespace IntegreNet.Tests.Integration
{
    [SetUpFixture]
    public class Initialization
    {
        private IntegreSql _integre;

        public static string Hash { get; private set; }

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _integre = new IntegreSql(Config.IntegreUrl);

            var schema = await File.ReadAllTextAsync("schema.sql");

            Hash = GetHash(schema);

            try
            {
                var template = await _integre.InitializeTemplateAsync(Hash);

                var connectionString = Helpers.CreateConnectionString(template);

                await using var connection = new NpgsqlConnection(connectionString);

                await connection.ExecuteAsync(schema);

                await _integre.FinalizeTemplateAsync(Hash);
            }
            catch (Exception e)
            {
                await _integre.DiscardTemplateAsync(Hash);

                Assert.Fail("Failed to setup the template database ({0})", e.Message);
            }
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _integre.DiscardTemplateAsync(Hash);
        }

        private static string GetHash(string text)
        {
            using var hash = MD5.Create();
            return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(text))
                .Select(x => x.ToString("x2")));
        }
    }
}