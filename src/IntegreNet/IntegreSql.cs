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
