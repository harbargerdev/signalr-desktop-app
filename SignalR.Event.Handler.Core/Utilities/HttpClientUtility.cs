using Newtonsoft.Json;
using SignalR.Event.Handler.Core.Utilities.Extensions;
using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SignalR.Event.Handler.Core.Utilities
{
    /// <inheritdoc />
    public class HttpClientUtility : IHttpClientUtility
    {
        private readonly HttpClient _httpClient;

        public HttpClientUtility()
        {
            _httpClient = new HttpClient();
        }

        public HttpClientUtility(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public T? ExecuteHttpGetSync<T>(string url) where T : ISerializable
        {
            if (url.IsNullOrEmptyTrimmed()) throw new ArgumentException("URL cannot be null, empty, or whitespace.", nameof(url));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            var httpResponse = _httpClient.Send(request);
            httpResponse.EnsureSuccessStatusCode();

            var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <inheritdoc />
        public async Task<T?> ExecuteHttpGetAsync<T>(string url) where T : ISerializable
        {
            if (url.IsNullOrEmptyTrimmed()) throw new ArgumentException("URL cannot be null, empty, or whitespace.", nameof(url));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            var httpResponse = await _httpClient.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <inheritdoc />
        public T? ExecuteHttpPostSync<T>(string url, ISerializable requestObject) where T : ISerializable
        {
            if (url.IsNullOrEmptyTrimmed()) throw new ArgumentException("URL cannot be null, empty, or whitespace.", nameof(url));
            if (requestObject == null) throw new ArgumentNullException(nameof(requestObject));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestObject), System.Text.Encoding.UTF8, "application/json")
            };

            var httpResponse = _httpClient.Send(request);
            httpResponse.EnsureSuccessStatusCode();

            var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <inheritdoc />
        public async Task<T?> ExecuteHttpPostAsync<T>(string url, ISerializable requestObject) where T : ISerializable
        {
            if (url.IsNullOrEmptyTrimmed()) throw new ArgumentException("URL cannot be null, empty, or whitespace.", nameof(url));
            if (requestObject == null) throw new ArgumentNullException(nameof(requestObject));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestObject), System.Text.Encoding.UTF8, "application/json")
            };

            var httpResponse = await _httpClient.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
