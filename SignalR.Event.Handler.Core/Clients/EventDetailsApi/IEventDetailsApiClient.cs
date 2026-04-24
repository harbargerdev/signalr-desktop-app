using System;
using System.Net.Http;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi.Responses;

namespace SignalR.Event.Handler.Core.Clients.EventDetailsApi
{
    public interface IEventDetailsApiClient
    {
        /// <summary>
        /// Gets the event details by user name.
        /// </summary>
        /// <param name="userName">The username to query events for</param>
        /// <returns>The active <see cref="EventDetailsResponse"/> entity returned by the API</returns>
        /// <throws cref="Exception">Throws an exception if the API call fails or returns an error response</throws>
        /// <throws cref="HttpRequestException">Throws an HttpRequestException if there is a network error or the API endpoint is unreachable</throws>
        /// <throws cref="ArgumentException">Throws an ArgumentException if the userName parameter is null, empty, or whitespace</throws>
        /// <throws cref="InvalidOperationException">Throws an InvalidOperationException if the API returns an unexpected response format or status code</throws>
        EventDetailsResponse GetEventDetailsByUserName(string userName);
    }
}
