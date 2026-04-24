using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SignalR.Event.Handler.Core.Utilities
{
    /// <summary>
    /// Defines utility methods for sending synchronous and asynchronous HTTP GET and POST requests with support for
    /// serializable request payloads and generic response deserialization.
    /// </summary>
    /// <remarks>Implementations of this interface provide methods to perform HTTP operations using either
    /// synchronous or asynchronous patterns. Methods that accept a request object require it to implement ISerializable
    /// for proper serialization. Response types must also implement ISerializable for deserialization. 
    /// Callers should handle exceptions related to invalid arguments, network failures, and timeouts as documented on each method.</remarks>
    public interface IHttpClientUtility
    {
        /// <summary>
        /// Sends a synchronous HTTP GET request to the specified URL and returns the deserialized response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into. Must implement ISerializable.</typeparam>
        /// <remarks>This method blocks the calling thread until the HTTP request completes. For
        /// asynchronous operations, use an appropriate asynchronous alternative.</remarks>
        /// <param name="url">The URL to which the HTTP GET request is sent. Cannot be null or empty.</param>
        /// <returns>An object of type T representing the deserialized response from the HTTP GET request, or null if deserialization fails.</returns>
        /// <throws cref="ArgumentException">Thrown when the url parameter is null, empty, or whitespace.</throws>
        /// <throws cref="HttpRequestException">Thrown when the HTTP request fails due to network issues, an invalid URL, or other request-related problems.</throws>
        /// <throws cref="TaskCanceledException">Thrown when the HTTP request times out.</throws>
        T? ExecuteHttpGetSync<T>(string url) where T : ISerializable;

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and returns the deserialized response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into. Must implement ISerializable.</typeparam>
        /// <param name="url">The URL to which the GET request is sent. Must be a valid, absolute URI.</param>
        /// <returns>A <see cref="Task"/> containing an object of type T representing the deserialized response from the HTTP GET request, or null if deserialization fails.</returns>
        /// <throws cref="ArgumentException">Thrown when the url parameter is null, empty, or whitespace.</throws>
        /// <throws cref="HttpRequestException">Thrown when the HTTP request fails due to network issues, an invalid URL, or other request-related problems.</throws>
        Task<T?> ExecuteHttpGetAsync<T>(string url) where T : ISerializable;

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the provided serializable request object and returns
        /// the deserialized response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into. Must implement ISerializable.</typeparam>
        /// <param name="url">The URL to which the HTTP POST request is sent. Must be a valid, absolute URI.</param>
        /// <param name="requestObject">The object to serialize and include in the body of the POST request. Must implement <see
        /// cref="ISerializable"/>.</param>
        /// <returns>An object of type T representing the deserialized response from the HTTP POST request, or null if deserialization fails.</returns>
        /// <throws cref="ArgumentException">Thrown when the url parameter is null, empty, or whitespace, or when the requestObject parameter is null.</throws>
        /// <throws cref="HttpRequestException">Thrown when the HTTP request fails due to network issues, an invalid URL, or other request-related problems.</throws>
        /// <throws cref="TaskCanceledException">Thrown when the HTTP request times out.</throws>
        T? ExecuteHttpPostSync<T>(string url, ISerializable requestObject) where T : ISerializable;

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the provided serializable request object as the
        /// payload and returns the deserialized response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into. Must implement ISerializable.</typeparam>
        /// <param name="url">The URL to which the HTTP POST request is sent. Must be a valid, absolute URI.</param>
        /// <param name="requestObject">The object to serialize and include in the body of the POST request. Must implement <see
        /// cref="ISerializable"/> and cannot be null.</param>
        /// <returns>A <see cref="Task"/> containing an object of type T representing the deserialized response from the HTTP POST request, or null if deserialization fails.</returns>
        /// <throws cref="ArgumentException">Thrown when the url parameter is null, empty, or whitespace, or when the requestObject parameter is null.</throws>
        /// <throws cref="HttpRequestException">Thrown when the HTTP request fails due to network issues, an invalid URL, or other request-related problems.</throws>
        Task<T?> ExecuteHttpPostAsync<T>(string url, ISerializable requestObject) where T : ISerializable;
    }
}
