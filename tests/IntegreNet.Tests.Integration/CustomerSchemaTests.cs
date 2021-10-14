using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NUnit.Framework;

namespace IntegreNet.Tests.Integration
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class CustomerSchemaTests
    {
        private const string BaseUrl = "http://localhost:6432/api/";
        private readonly IntegreSql _integre = new(BaseUrl);

        [Test]
        public async Task Count_WhenAllAreDeleted_IsZero()
        {
            var database = await _integre.GetTestDatabaseAsync(Initialization.Hash);

            var connectionString = Helpers.CreateConnectionString(database);

            await using var connection = new NpgsqlConnection(connectionString);

            var countBefore = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countBefore, Is.EqualTo(3));

            await connection.ExecuteAsync("DELETE FROM customers");

            var countAfter = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countAfter, Is.EqualTo(0));
        }

        [Test]
        public async Task Count_WhenInserted_IsCorrect()
        {
            var database = await _integre.GetTestDatabaseAsync(Initialization.Hash);

            var connectionString = Helpers.CreateConnectionString(database);

            await using var connection = new NpgsqlConnection(connectionString);

            var countBefore = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countBefore, Is.EqualTo(3));

            await connection.ExecuteAsync("INSERT INTO customers(name) VALUES ('Joe'), ('Jane')");

            var countAfter = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countAfter, Is.EqualTo(5));

            var names = (await connection.QueryAsync<string>("SELECT name FROM customers")).ToList();

            Assert.That(names, Is.Not.Empty);
            Assert.That(names.Count, Is.EqualTo(5));
            Assert.That(names, Is.EquivalentTo(new[] { "Bob", "Stuart", "Steve", "Joe", "Jane" }));
        }

        [Test]
        public async Task Count_WhenSomeDeleted_IsCorrect()
        {
            var database = await _integre.GetTestDatabaseAsync(Initialization.Hash);

            var connectionString = Helpers.CreateConnectionString(database);

            await using var connection = new NpgsqlConnection(connectionString);

            var countBefore = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countBefore, Is.EqualTo(3));

            await connection.ExecuteAsync("DELETE FROM customers WHERE name LIKE 'S%'");

            var countAfter = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM customers");

            Assert.That(countAfter, Is.EqualTo(1));

            var names = (await connection.QueryAsync<string>("SELECT name FROM customers")).ToList();

            Assert.That(names, Is.Not.Empty);
            Assert.That(names.Count, Is.EqualTo(1));
            Assert.That(names, Is.EquivalentTo(new[] { "Bob" }));
        }
    }
}