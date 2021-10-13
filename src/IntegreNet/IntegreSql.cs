using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegreNet.Model;

namespace IntegreNet
{
    public class IntegreSql
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

        public async Task<Template> InitializeTemplateAsync(string hash)
        {
            var message = await _http.PostAsJsonAsync("v1/templates", new { hash = hash }).ConfigureAwait(false);

            switch (message.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await message.Content.ReadFromJsonAsync<Template>().ConfigureAwait(false);
                case (HttpStatusCode)423:
                    throw new Exception("Some other process has already recreated a PostgreSQL template database for this hash (or is currently doing it)");
                case HttpStatusCode.ServiceUnavailable:
                    throw new Exception("Service unavailable");
                default:
                    throw new Exception($"Unexpected status code: {message.StatusCode}");
            }
        }

        public async Task FinalizeTemplateAsync(string hash)
        {
            var message = await _http.PutAsync($"v1/templates/{hash}", null).ConfigureAwait(false);

            switch (message.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return;
                default:
                    throw new Exception($"Unexpected status code: {message.StatusCode}");
            }
        }

        public async Task DiscardTemplateAsync(string hash)
        {
            var message = await _http.DeleteAsync($"v1/templates/{hash}").ConfigureAwait(false);

            switch (message.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return;
                default:
                    throw new Exception($"Unexpected status code: {message.StatusCode}");
            }
        }

        public async Task<Template> GetTestDatabaseAsync(string hash)
        {
            var message  = await _http.GetAsync($"v1/templates/{hash}/tests").ConfigureAwait(false);

            switch (message.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await message.Content.ReadFromJsonAsync<Template>().ConfigureAwait(false);
                case HttpStatusCode.NotFound:
                    throw new Exception("Template not found. Make sure you successfully called InitializeTemplate");
                case HttpStatusCode.Gone:
                    throw new Exception("There was an error during test setup with our fixtures, someone called DiscardTemplate, thus this template cannot be used");
                case HttpStatusCode.ServiceUnavailable:
                    throw new Exception("Service unavailable");
                default:
                    throw new Exception($"Unexpected status code: {message.StatusCode}");
            }
        }
    }
}
