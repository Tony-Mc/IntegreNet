using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegreNet.Exceptions;
using IntegreNet.Model;
using static IntegreNet.Try;

namespace IntegreNet
{
    public sealed class IntegreSql
    {
        private readonly HttpClient _http;

        public IntegreSql(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public IntegreSql(string baseUrl, HttpMessageHandler handler)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        /// <summary>
        /// Initializes a new template database. This method is intended to be called once per test runner process.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="InitializeTemplateAsync"/> use the returned information to connect to a database, apply schema migrations, seed fixture data and then call <see cref="FinalizeTemplateAsync"/>.
        /// If you encounter an exception call <see cref="DiscardTemplateAsync"/> to discard a failed template.
        /// </remarks>
        /// <param name="hash">The hash of your database migration/fixture files.</param>
        /// <returns>Initialized database <see cref="Template"/>.</returns>
        /// <exception cref="TemplateLockedException">Some other process has already recreated a PostgreSQL template database for this hash.</exception>
        /// <exception cref="ServiceUnavailableException">Service unavailable, there may be connection issues between IntegreSQL and the database.</exception>
        /// <exception cref="IntegreException">Unexpected status returned.</exception>
        public async Task<Template> InitializeTemplateAsync(string hash)
        {
            return await TryHandle(async () =>
            {
                var message = await _http.PostAsJsonAsync("v1/templates", new { hash = hash }).ConfigureAwait(false);

                switch (message.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await message.Content.ReadFromJsonAsync<Template>().ConfigureAwait(false);
                    case (HttpStatusCode)423:
                        throw new TemplateLockedException();
                    case HttpStatusCode.ServiceUnavailable:
                        throw new ServiceUnavailableException();
                    default:
                        throw new IntegreException($"Unexpected status code: {message.StatusCode}");
                }
            });
        }

        /// <summary>
        /// Marks database template for provided hash as finalized allowing retrieval of test databases via <see cref="GetTestDatabaseAsync"/>.
        /// </summary>
        /// <remarks>
        /// If you encounter an exception call <see cref="DiscardTemplateAsync"/> to discard a failed template.
        /// </remarks>
        /// <param name="hash">The hash of your database migration/fixture files.</param>
        /// <exception cref="IntegreException">Unexpected status returned.</exception>
        public async Task FinalizeTemplateAsync(string hash)
        {
            await TryHandle(async () =>
            {
                var message = await _http.PutAsync($"v1/templates/{hash}", null).ConfigureAwait(false);

                switch (message.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return;
                    default:
                        throw new IntegreException($"Unexpected status code: {message.StatusCode}");
                }
            });
        }

        /// <summary>
        /// Discards a template for provided hash.
        /// </summary>
        /// <param name="hash">The hash of your database migration/fixture files.</param>
        /// <exception cref="IntegreException">Unexpected status returned.</exception>
        public async Task DiscardTemplateAsync(string hash)
        {
            await TryHandle(async () =>
            {
                var message = await _http.DeleteAsync($"v1/templates/{hash}").ConfigureAwait(false);

                switch (message.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return;
                    default:
                        throw new IntegreException($"Unexpected status code: {message.StatusCode}");
                }
            });
        }

        /// <summary>
        /// Retrieves a fully isolated PostgreSQL database from a migrated/seeded template for this hash. Use this database in your test.
        /// </summary>
        /// <param name="hash">The hash of your database migration/fixture files.</param>
        /// <returns>A ready to use database <see cref="Template"/>.</returns>
        /// <exception cref="TemplateNotFoundException">Template not found. Make sure it is initialized using <see cref="InitializeTemplateAsync"/></exception>
        /// <exception cref="TemplateDiscardedException">Template was discarded and cannot be used.</exception>
        /// <exception cref="ServiceUnavailableException">Service unavailable, there may be connection issues between IntegreSQL and the database.</exception>
        /// <exception cref="IntegreException">Unexpected status returned.</exception>
        public async Task<Template> GetTestDatabaseAsync(string hash)
        {
            return await TryHandle(async () =>
            {
                var message = await _http.GetAsync($"v1/templates/{hash}/tests").ConfigureAwait(false);

                switch (message.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await message.Content.ReadFromJsonAsync<Template>().ConfigureAwait(false);
                    case HttpStatusCode.NotFound:
                        throw new TemplateNotFoundException();
                    case HttpStatusCode.Gone:
                        throw new TemplateDiscardedException();
                    case HttpStatusCode.ServiceUnavailable:
                        throw new ServiceUnavailableException();
                    default:
                        throw new IntegreException($"Unexpected status code: {message.StatusCode}");
                }
            });
        }
    }
}
