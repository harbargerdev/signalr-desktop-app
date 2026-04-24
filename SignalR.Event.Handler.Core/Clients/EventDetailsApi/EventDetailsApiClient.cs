using Microsoft.Extensions.Options;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi.Responses;
using SignalR.Event.Handler.Core.Configuration;
using SignalR.Event.Handler.Core.Utilities;
using System;

namespace SignalR.Event.Handler.Core.Clients.EventDetailsApi
{
    public class EventDetailsApiClient : IEventDetailsApiClient
    {
        private readonly IHttpClientUtility _httpClientUtility;
        private readonly ApiSettings _apiSettings;
        private readonly RetryPolicy _retryPolicy;

        public EventDetailsApiClient(IHttpClientUtility httpClientUtility, IOptions<ApiSettings> apiSettings)
        {
            _httpClientUtility = httpClientUtility;
            _apiSettings = apiSettings.Value;

            // Initialize retry policy from configuration
            _retryPolicy = new RetryPolicy
            {
                MaxRetries = _apiSettings.MaxRetries,
                InitialDelayMs = _apiSettings.InitialRetryDelayMs,
                BackoffMultiplier = _apiSettings.RetryBackoffMultiplier,
                MaxDelayMs = _apiSettings.MaxRetryDelayMs
            };
        }

        /// <inheritdoc />
        public EventDetailsResponse GetEventDetailsByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username cannot be null, empty, or whitespace.", nameof(userName));
            }

            // Use _apiSettings.EventDetailsApiUrl to make the API call with retry logic
            var apiUrl = $"{_apiSettings.EventDetailsApiUrl}/query/{userName}/";

            var response = _retryPolicy.Execute(() => _httpClientUtility.ExecuteHttpGetSync<EventDetailsResponse>(apiUrl));

            if (response == null)
            {
                throw new InvalidOperationException("Failed to deserialize response from API.");
            }

            return response;
        }
    }
}
