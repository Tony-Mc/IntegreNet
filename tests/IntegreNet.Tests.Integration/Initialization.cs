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
        private const string BaseUrl = "http://localhost:6432/api/";

        private IntegreSql _integre;
        public static string Hash;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _integre = new IntegreSql(BaseUrl);

            var schema = await File.ReadAllTextAsync("schema.sql");

            Hash = GetHash(schema);

            try
            {
                var template = await _integre.InitializeTemplateAsync(Hash);

                var a = new NpgsqlConnectionStringBuilder
                {
                    Host = "localhost",
                    //Host = template.Database.Config.Host,
                    Port = template.Database.Config.Port,
                    Username = template.Database.Config.Username,
                    Password = template.Database.Config.Password,
                    Database = template.Database.Config.Database,
                    Pooling = false // turn pooling off so connections are truly closed, otherwise IntegreSQL will detect open connections and fail to return a test database
                };

                await using var conn = new NpgsqlConnection(a.ToString());

                await conn.ExecuteAsync(schema);

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

    [TestFixture]
    [Parallelizable(ParallelScope.Children)] // Indicate that tests in this fixture can be run in parallel
    public class Tests
    {
        private const string BaseUrl = "http://localhost:6432/api/";

        private readonly IntegreSql _integre = new(BaseUrl);

        [Test]
        public async Task Test1()
        {
            var database = await _integre.GetTestDatabaseAsync(Initialization.Hash);

            var a = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = database.Database.Config.Port,
                Username = database.Database.Config.Username,
                Password = database.Database.Config.Password,
                Database = database.Database.Config.Database,
                Pooling = false // turn pooling off so connections are truly closed, otherwise IntegreSQL will detect open connections and fail to return a test database
            };

            await using var conn = new NpgsqlConnection(a.ToString());

            var countBefore = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countBefore, Is.EqualTo(3));

            await conn.ExecuteAsync("INSERT INTO customers(name) VALUES ('Joe')");

            var countAfter = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countAfter, Is.EqualTo(4));
        }

        [Test]
        public async Task Test2()
        {
            var database = await _integre.GetTestDatabaseAsync(Initialization.Hash);

            var a = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = database.Database.Config.Port,
                Username = database.Database.Config.Username,
                Password = database.Database.Config.Password,
                Database = database.Database.Config.Database,
                Pooling = false // turn pooling off so connections are truly closed, otherwise IntegreSQL will detect open connections and fail to return a test database
            };

            await using var conn = new NpgsqlConnection(a.ToString());

            var countBefore = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countBefore, Is.EqualTo(3));

            await conn.ExecuteAsync("DELETE FROM customers WHERE name LIKE 'S%'");

            var countAfter = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countAfter, Is.EqualTo(1));
        }
    }
}